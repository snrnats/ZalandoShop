using System.Linq;
using ZalandoShop.UWP.Model.Entities.ZalandoShop;
using CommonEntries = ZalandoShop.UWP.Model.Entities.Common;

namespace ZalandoShop.UWP.Services.ZalandoShop
{
    public static class Converter
    {
        public static CommonEntries.Article Convert(Article article)
        {
            return new CommonEntries.Article
            {
                Name = article.Name,
                Color = article.Color,
                Units = article.Units.Select(Convert).ToList(),
                Media = new CommonEntries.Media
                {
                    Images = article.Media.Images.Select(image => new CommonEntries.Image
                    {
                        SmallImage = image.SmallUrl,
                        MediumImage = image.MediumUrl,
                        LargeImage = image.LargeUrl
                    }).ToList()
                }
            };
        }

        public static CommonEntries.Price Convert(ArticlePrice price)
        {
            var result = new CommonEntries.Price
            {
                Amount = price.Value,
                Currency = price.Currency,
                Formatted = price.Formatted
            };
            return result;
        }

        public static CommonEntries.ArticleUnit Convert(ArticleUnit unit)
        {
            var result = new CommonEntries.ArticleUnit
            {
                Price = Convert(unit.Price),
                Size = unit.Size
            };
            return result;
        }

        public static CommonEntries.Facet Convert(Facet facet)
        {
            var result = new CommonEntries.Facet
            {
                Filter = facet.Filter,
                Values = facet.Facets.Select(Convert).ToList()
            };
            return result;
        }

        public static CommonEntries.FacetValue Convert(FacetValue facetValue)
        {
            var result = new CommonEntries.FacetValue
            {
                Key = facetValue.Key,
                DisplayName = facetValue.DisplayName
            };
            return result;
        }

        public static CommonEntries.PaginationResponse<CommonEntries.Article> Convert(PaginationResponse<Article> pagination)
        {
            var result = new CommonEntries.PaginationResponse<CommonEntries.Article>
            {
                Content = pagination.Content.Select(Convert).ToList(),
                Page = pagination.Page,
                Size = pagination.Size,
                TotalElements = pagination.TotalElements,
                TotalPages = pagination.TotalPages
            };
            return result;
        }
    }
}
