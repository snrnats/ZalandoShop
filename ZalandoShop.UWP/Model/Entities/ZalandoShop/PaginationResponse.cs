using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZalandoShop.UWP.Model.Entities.ZalandoShop
{
    public class PaginationResponse<T> where T : class
    {
        [JsonProperty("totalElements")]
        public int TotalElements { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("content")]
        public List<T> Content { get; set; }
    }
}
