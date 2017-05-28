using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZalandoShop.UWP.Model.Entities.Common
{
    public class Article
    {
        public Media Media { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public List<ArticleUnit> Units { get; set; }

        public string PriceRange
        {
            get
            {
                var minAmount = double.MaxValue;
                var maxAmount = double.MinValue;
                string min = null, max = null;
                foreach (var unit in Units)
                {
                    var price = unit.Price.Amount;
                    if (price < minAmount)
                    {
                        minAmount = price;
                        min = unit.Price.Formatted;
                    }
                    if (price > maxAmount)
                    {
                        maxAmount = price;
                        max = unit.Price.Formatted;
                    }
                }
                return minAmount != maxAmount ? $"{min} - {max}" : min;
            }
        }

        public string SizeRange
        {
            get
            {
                return Units.Select(unit => unit.Size).Aggregate(new StringBuilder(), (builder, size)=> {
                    if (!string.IsNullOrWhiteSpace(size))
                    {
                        builder.Append(size).Append(", ");
                    }
                    return builder;
                }, builder =>
                {
                    if (builder.Length > 0)
                    {
                        builder.Remove(builder.Length - 3, 2);
                    }
                    return builder.ToString();
                });
            }
        }
    }
}
