using System.Collections.Generic;
using ZalandoShop.UWP.Helpers.QueryString;

namespace ZalandoShop.UWP.Model.Entities.Common
{
    public class FacetsRequest
    {
        [QueryParameterMap]
        public Dictionary<string, IList<string>> Filters { get; set; }
    }
}
