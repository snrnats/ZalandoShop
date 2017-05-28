using System.Collections.Generic;

namespace ZalandoShop.UWP.Model.Entities.Common
{
    public class Facet
    {
        public string Filter { get; set; }
        public IList<FacetValue> Values { get; set; }
    }
}
