using System.Collections.Generic;

namespace ZalandoShop.UWP.Model.Entities.Common
{
    public class PaginationResponse<T> where T : class
    {
        public int TotalElements { get; set; }

        public int TotalPages { get; set; }

        public int Page { get; set; }

        public int Size { get; set; }

        public List<T> Content { get; set; }
    }
}
