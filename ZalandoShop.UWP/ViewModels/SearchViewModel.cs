using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json;
using Nito.Mvvm;
using ZalandoShop.UWP.Helpers;
using ZalandoShop.UWP.Model.Entities.Common;
using ZalandoShop.UWP.Platform;
using ZalandoShop.UWP.Services;
using ZalandoShop.UWP.Services.CommonApi;
using ZalandoShop.UWP.Services.CommonApi.Exceptions;
using ZalandoShop.UWP.ViewModels.Common;
using ZalandoShop.UWP.Views;

namespace ZalandoShop.UWP.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private const string FacetsCacheFolder = "FacetsCache";
        private const string CategoryFilter = "category";
        private const string BrandFilter = "brand";
        private const string BrandFamilyFilter = "brandFamily";
        private const string GenderFilter = "gender";
        private static readonly string[] SuggestionFilters = {CategoryFilter, BrandFilter, BrandFamilyFilter};
        private static readonly Dictionary<string, IList<string>> PrecompiledFacets;

        private readonly FacetsRequest _facetsRequest;
        private readonly NavigationServiceEx _navigationService;
        private readonly ProgressService _progressService;
        private readonly IShopApiService _shopService;
        private List<Facet> _facets;
        private int _genderIndex;
        private string _searchQuery;
        private List<FacetValue> _searchSuggestions;
        private AsyncCommand _submitQueryCommand;
        private AsyncCommand _submitSelectionCommand;
        private RelayCommand _updateSuggestionsCommand;

        static SearchViewModel()
        {
            PrecompiledFacets = new Dictionary<string, IList<string>> {{GenderFilter, new List<string> {"male", "female"}}};
        }

        public SearchViewModel(IShopApiService shopService, NavigationServiceEx navigationService, ProgressService progressService)
        {
            _shopService = shopService;
            _navigationService = navigationService;
            _progressService = progressService;
            PropertyChanged += OnPropertyChanged;
            _facetsRequest = new FacetsRequest {Filters = new Dictionary<string, IList<string>> {{GenderFilter, new List<string> {GenderKey}}}};
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => Set(ref _searchQuery, value);
        }

        public int GenderIndex
        {
            get => _genderIndex;
            set => Set(ref _genderIndex, value);
        }

        public string GenderKey => PrecompiledFacets[GenderFilter][GenderIndex];

        public List<FacetValue> SearchSuggestions
        {
            get => _searchSuggestions;
            set => Set(ref _searchSuggestions, value);
        }

        public List<Facet> Facets
        {
            get => _facets;
            set => Set(ref _facets, value);
        }

        public List<string> Genders { get; } = PrecompiledFacets[GenderFilter].Select(s => s.GetLocalized()).ToList();

        public RelayCommand UpdateSuggestionsCommand => _updateSuggestionsCommand ?? (_updateSuggestionsCommand = new RelayCommand(() =>
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery) && Facets != null)
            {
                SearchSuggestions = Facets.Where(facet => SuggestionFilters.Contains(facet.Filter)).SelectMany(facet => facet.Values)
                    .Where(value => value.DisplayName.StartsWith(SearchQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            else
            {
                SearchSuggestions = new List<FacetValue>();
            }
        }));

        public AsyncCommand SubmitSelectionCommand => _submitSelectionCommand ?? (_submitSelectionCommand = new AsyncCommand(async e =>
        {
            var selectedFacet = Facets.FirstOrDefault(facet => facet.Values.Contains(e));
            if (selectedFacet == null)
            {
                return;
            }
            var filters = new Dictionary<string, IList<string>>
            {
                {selectedFacet.Filter, new List<string> {((FacetValue) e).Key}},
                {GenderFilter, new List<string> {GenderKey}}
            };
            var request = new ArticlesSearchRequest {Filters = filters};
            var parameterId = await SetNavigationParameter(request);
            _navigationService.Navigate(_navigationService.GetNameOfRegisteredPage(typeof(ArticlesPage)), parameterId);
        }));

        public AsyncCommand SubmitQueryCommand => _submitQueryCommand ?? (_submitQueryCommand = new AsyncCommand(async e =>
        {
            var request = new ArticlesSearchRequest {Query = (string) e};
            var parameterId = await SetNavigationParameter(request);
            _navigationService.Navigate(_navigationService.GetNameOfRegisteredPage(typeof(ArticlesPage)), parameterId);
        }));

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GenderIndex))
            {
                _facetsRequest.Filters[GenderFilter] = new List<string> {GenderKey};
                await UpdateFacets().ConfigureAwait(false);
            }
        }

        private int GetFacetsId(FacetsRequest request)
        {
            return request.Filters.Aggregate(0, (i, pair) => i ^ pair.Key.GetHashCode() ^ pair.Value.Aggregate(0, (i1, s) => i1 ^ s.GetHashCode()));
        }

        private async Task UpdateFacets()
        {
            var facetsId = GetFacetsId(_facetsRequest);
            var cachedFacets = await TryGetCachedFacets(facetsId);
            if (cachedFacets != null)
            {
                Facets = cachedFacets;
            }
            else
            {
                using (_progressService.BeginUiOperation())
                {
                    Facets = await LoadServerFacets(facetsId);
                }
            }
            UpdateSuggestionsCommand.Execute(null);
        }

        private async Task<List<Facet>> TryGetCachedFacets(int facetsId)
        {
            try
            {
                var cacheFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(FacetsCacheFolder, CreationCollisionOption.OpenIfExists);
                var cachedFile = await cacheFolder.TryGetItemAsync(facetsId.ToString()) as StorageFile;
                if (cachedFile != null)
                {
                    var content = await FileIO.ReadTextAsync(cachedFile);
                    return JsonConvert.DeserializeObject<List<Facet>>(content);
                }
            }
            catch (JsonException e)
            {
                Logger.Error("Failed to deserialize cached facets", e);
            }
            catch (IOException e)
            {
                Logger.Error("Failed to read cached facets", e);
            }

            return null;
        }

        private async Task<List<Facet>> LoadServerFacets(int facetsId)
        {
            List<Facet> result = null;
            try
            {
                result = await FuncEx.Make(() => _shopService.GetFacets(_facetsRequest)).RetryWhenException<List<Facet>, ApiException>();
                var cacheFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(FacetsCacheFolder, CreationCollisionOption.OpenIfExists);
                await cacheFolder.WriteTextToFileAsync(JsonConvert.SerializeObject(Facets), facetsId.ToString());
            }
            catch (ApiException apiException)
            {
                Logger.Error("Failed to update facets", apiException);
            }
            return result;
        }

        public override async void Activate(object parameter)
        {
            await UpdateFacets().ConfigureAwait(false);
        }
    }
}
