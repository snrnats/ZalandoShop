using System.Collections.Generic;
using ZalandoShop.UWP.Helpers.QueryString;

namespace ZalandoShop.UWP.Model.Entities.Common
{
    public class ArticlesSearchRequest
    {
        [QueryParameterMap]
        public Dictionary<string, IList<string>> Filters { get; set; }

        [QueryParameter("fullText")]
        public string Query { get; set; }
    }
}
