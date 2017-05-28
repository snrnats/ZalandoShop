using System;
using Nito.Mvvm;
using ZalandoShop.UWP.Model.Entities.Common;
using ZalandoShop.UWP.Platform;
using ZalandoShop.UWP.Services;
using ZalandoShop.UWP.Services.CommonApi;
using ZalandoShop.UWP.ViewModels.Common;

namespace ZalandoShop.UWP.ViewModels
{
    public class ArticlesViewModel : ViewModelBase
    {
        private readonly NavigationServiceEx _navigationService;
        private readonly ProgressService _progressService;
        private readonly IShopApiService _shopApiService;
        private IncrementalLoadingCollection<ArticlesIncrementalSource, Article> _articles;
        private DataAvailabilityState _dataState;

        private AsyncCommand _reloadDataCommand;

        public ArticlesViewModel(NavigationServiceEx navigationService, IShopApiService shopApiService, ProgressService progressService)
        {
            _navigationService = navigationService;
            _shopApiService = shopApiService;
            _progressService = progressService;
        }

        public IncrementalLoadingCollection<ArticlesIncrementalSource, Article> Articles
        {
            get => _articles;
            set => Set(ref _articles, value);
        }

        public DataAvailabilityState DataState
        {
            get => _dataState;
            set => Set(ref _dataState, value);
        }

        public AsyncCommand ReloadDataCommand => _reloadDataCommand ?? (_reloadDataCommand = new AsyncCommand(() => { return Articles.RefreshAsync(); }));

        public override async void Activate(object parameter)
        {
            if (parameter is string parameterId)
            {
                var searchRequest = await GetNavigationParameter<ArticlesSearchRequest>(parameterId);
                if (searchRequest != null)
                {
                    DataState = DataAvailabilityState.NotReady;
                    Articles = new IncrementalLoadingCollection<ArticlesIncrementalSource, Article>(
                        new ArticlesIncrementalSource(_shopApiService, _progressService, searchRequest), onEndLoading: OnEndLoading, onError: OnError);
                }
                else
                {
                    Logger.Warn("Navigating back because can't get navigation parameter");
                    _navigationService.GoBack();
                }
            }
            else
            {
                Logger.Warn("Navigating back because no parameter id was provided");
                _navigationService.GoBack();
            }
        }

        private void OnEndLoading()
        {
            if (Articles.Count != 0)
            {
                DataState = DataAvailabilityState.Available;
            }
        }

        private void OnError(Exception exception)
        {
            if (Articles.Count == 0)
            {
                DataState = DataAvailabilityState.Unavailable;
            }
        }
    }
}
