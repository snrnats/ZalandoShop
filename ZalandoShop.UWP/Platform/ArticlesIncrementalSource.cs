using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using MetroLog;
using Microsoft.Toolkit.Uwp;
using ZalandoShop.UWP.Helpers;
using ZalandoShop.UWP.Model.Entities.Common;
using ZalandoShop.UWP.Services;
using ZalandoShop.UWP.Services.CommonApi;
using ZalandoShop.UWP.Services.CommonApi.Exceptions;

namespace ZalandoShop.UWP.Platform
{
    public class ArticlesIncrementalSource : IIncrementalSource<Article>
    {
        private static readonly ILogger Logger = LogManagerFactory.DefaultLogManager.GetLogger<ArticlesIncrementalSource>();
        private readonly ProgressService _progressService;
        private readonly ArticlesSearchRequest _searchRequest;
        private readonly IShopApiService _shopApiService;

        public ArticlesIncrementalSource(IShopApiService shopApiService, ProgressService progressService, ArticlesSearchRequest searchRequest)
        {
            _shopApiService = shopApiService;
            _searchRequest = searchRequest;
            _progressService = progressService;
        }

        public async Task<IEnumerable<Article>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new CancellationToken())
        {
            using (pageIndex == 0 ? _progressService.BeginUiOperation(true) : _progressService.BeginBackgroundOperation(true))
            {
                try
                {
                    var paginationResponse = await FuncEx.Make(() => _shopApiService.GetArticles(new PaginationRequest<ArticlesSearchRequest>
                        {
                            Page = pageIndex + 1,
                            PageSize = pageSize,
                            Request = _searchRequest
                        })
                    ).RetryWhenException<PaginationResponse<Article>, ApiException>().ConfigureAwait(false);
                    return paginationResponse.Content;
                }
                catch (ApiException e)
                {
                    Logger.Error($"Failed to load article items. Page {pageIndex}, pageSize {pageSize}", e);
                    throw;
                }
            }
        }
    }
}
