#region using

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

#endregion

namespace TestMVideo
{
    public class ProductsService
    {
        private readonly ProductsStorage productsStorage;

        public ProductsService(IConfiguration configuration)
        {
            var dataParser = new DataParser();
            var inputLines = dataParser.ParseCsvFile(configuration["DataFilePath"]);
            productsStorage = new ProductsStorage(inputLines);
        }

        public IEnumerable<string> GetRecommendedProductsIds(string productId)
        {
            var intId = productsStorage.GetProductsIntId(productId);
            return intId == -1
                ? Array.Empty<string>()
                : productsStorage.ProductsWithRecommendations[intId]
                    .SelectMany(x => x.Value)
                    .Select(x => productsStorage.GetProductsStringId(x));
        }

        public IEnumerable<string> GetRecommendedProductsIdsWithMinRank(string productId, float minRank)
        {
            var intId = productsStorage.GetProductsIntId(productId);
            return intId == -1
                ? Array.Empty<string>()
                : productsStorage.ProductsWithRecommendations[intId]
                    .Where(x => x.Key >= minRank)
                    .SelectMany(x => x.Value)
                    .Select(x => productsStorage.GetProductsStringId(x));
        }
    }
}