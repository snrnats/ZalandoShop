using ZalandoShop.UWP.Helpers.QueryString;

namespace ZalandoShop.UWP.Model.Entities.Common
{
    public class PaginationRequest<T>
    {
        [InlineQueryParameter]
        public T Request { get; set; }

        [QueryParameter("page")]
        public int Page { get; set; }

        [QueryParameter("pageSize")]
        public int PageSize { get; set; }
    }
}
