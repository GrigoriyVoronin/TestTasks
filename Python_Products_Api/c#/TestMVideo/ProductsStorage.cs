#region using

using System.Collections.Generic;
using TestMVideo.Models;

#endregion

namespace TestMVideo
{
    public class ProductsStorage
    {
        private const int ProductsCount = 1_200_000;
        private readonly List<string> products = new(ProductsCount);
        private readonly Dictionary<string, int> productsStr = new(ProductsCount);

        public ProductsStorage(IEnumerable<InputLine> inputLines)
        {
            foreach (var inputLine in inputLines)
                AddLineToStorage(inputLine);
        }

        public Dictionary<int, Dictionary<float, List<int>>> ProductsWithRecommendations { get; } = new(ProductsCount);

        public string GetProductsStringId(int intId)
        {
            return products[intId];
        }

        public int GetProductsIntId(string stringId)
        {
            return productsStr.GetValueOrDefault(stringId, -1);
        }

        private void AddLineToStorage(InputLine inputLine)
        {
            var productIntId = InitProduct(inputLine.ProductId);
            var recommendationIntId = InitProduct(inputLine.RecommendationId);
            ProductsWithRecommendations.TryAdd(productIntId, new Dictionary<float, List<int>>());
            ProductsWithRecommendations[productIntId].TryAdd(inputLine.Rank, new List<int>());
            ProductsWithRecommendations[productIntId][inputLine.Rank].Add(recommendationIntId);
        }

        private int InitProduct(string productsId)
        {
            if (productsStr.ContainsKey(productsId))
                return productsStr[productsId];

            productsStr[productsId] = products.Count;
            products.Add(productsId);
            return products.Count - 1;
        }
    }
}