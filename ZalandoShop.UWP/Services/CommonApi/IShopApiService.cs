using System.Collections.Generic;
using System.Threading.Tasks;
using ZalandoShop.UWP.Model.Entities.Common;
using ZalandoShop.UWP.Services.CommonApi.Exceptions;

namespace ZalandoShop.UWP.Services.CommonApi
{
    /// <summary>
    ///     Common iterface for shop api clients
    /// </summary>
    public interface IShopApiService
    {
        /// <summary></summary>
        /// <exception cref="ServerException">
        ///     Thrown when server responded with unsuccessful status code or response can't be
        ///     deserialized.
        /// </exception>
        /// <exception cref="NetworkException">Thrown when client can't reach the server because of network issues.</exception>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        Task<PaginationResponse<Article>> GetArticles(PaginationRequest<ArticlesSearchRequest> requestParams);

        /// <summary></summary>
        /// <exception cref="ServerException">
        ///     Thrown when server responded with unsuccessful status code or response can't be
        ///     deserialized.
        /// </exception>
        /// <exception cref="NetworkException">Thrown when client can't reach the server because of network issues.</exception>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        Task<List<Facet>> GetFacets(FacetsRequest requestParams);
    }
}
