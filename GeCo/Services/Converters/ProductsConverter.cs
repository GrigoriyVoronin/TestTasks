using GeCoTest.Models.API;
using GeCoTest.Models.API.Requests;

namespace GeCoTest.Services.Converters
{
    public class ProductsConverter
    {
        public Product ConvertFromRequest(ProductFromRequest product)
        {
            return new Product
            {
                Id = product.Id,
                Comment = product.Comment,
                Description = product.Description,
                DiscountAmount = GetZeroForEmptyString(product.DiscountAmount),
                Name = product.Name,
                PaidPrice = GetZeroForEmptyString(product.PaidPrice),
                Quantity = GetZeroForEmptyString(product.Quantity),
                RemoteCode = product.RemoteCode,
                UnitPrice = GetZeroForEmptyString(product.UnitPrice),
                VatPercentage = GetZeroForEmptyString(product.VatPercentage)
            };
        }

        private decimal GetZeroForEmptyString(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? 0
                : decimal.Parse(input);
        }
    }
}