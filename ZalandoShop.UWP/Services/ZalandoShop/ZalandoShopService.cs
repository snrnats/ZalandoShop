using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using ZalandoShop.UWP.Helpers.QueryString;
using ZalandoShop.UWP.Model.Entities.ZalandoShop;
using ZalandoShop.UWP.Services.CommonApi;
using CommonEntries = ZalandoShop.UWP.Model.Entities.Common;

namespace ZalandoShop.UWP.Services.ZalandoShop
{
    public class ZalandoShopService : ApiServiceBase, IShopApiService
    {
        public const string BaseUrl = "https://api.zalando.com";
        public const string ArticleEndpoint = "articles";
        public const string FacetsEndpoint = "facets";

        public async Task<CommonEntries.PaginationResponse<CommonEntries.Article>> GetArticles(CommonEntries.PaginationRequest<CommonEntries.ArticlesSearchRequest> requestParams)
        {
            var response = await SendAsync<PaginationResponse<Article>>(HttpMethod.Get, new[] {BaseUrl, ArticleEndpoint}, QueryHelper.QueryString(requestParams)).ConfigureAwait(false);
            var result = Converter.Convert(response);
            return result;
        }

        public async Task<List<CommonEntries.Facet>> GetFacets(CommonEntries.FacetsRequest requestParams)
        {
            var response = await SendAsync<List<Facet>>(HttpMethod.Get, new[] {BaseUrl, FacetsEndpoint}, QueryHelper.QueryString(requestParams)).ConfigureAwait(false);
            var result = response.Select(Converter.Convert).ToList();
            return result;
        }
    }
}
