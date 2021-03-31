#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace Fridge
{
    public class RacksByProductTypeComparer : IComparer<Rack>
    {
        public int Compare(Rack x, Rack y)
        {
            if (x == null || y == null)
                return 0;

            return x.ProductType.Id.CompareTo(y.ProductType.Id);
        }
    }

    public class WrongFormatException : Exception
    {
        public WrongFormatException(string wrongInput)
            : base($"Неверный формат ввода в строке: {wrongInput}")
        {
        }
    }

    public class Rack : IComparable<Rack>
    {
        private int? size;

        public Rack(int weight, int height)
        {
            Weight = weight;
            Height = height;
        }

        public int Size => size ??= Weight * Height;
        public int Weight { get; }
        public int Height { get; }

        public ProductType ProductType { get; private set; }
        public int Quantity { get; private set; }

        public int CompareTo(Rack other)
        {
            return Size.CompareTo(other.Size);
        }


        public void PutProduct(ProductType productType, int quantity)
        {
            if (quantity > Size)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            ProductType = productType;
            Quantity = quantity;
        }

        public override int GetHashCode()
        {
            return Size;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var printedCount = 0;
            var productTypeString = ProductType.ToString();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Weight; j++)
                {
                    var symbol = printedCount++ < Quantity
                        ? productTypeString
                        : "-";
                    builder.Append($"{symbol} ");
                }

                builder.Remove(builder.Length - 1, 1);
                builder.Append('\n');
            }

            return builder.ToString();
        }
    }

    public class Product : IComparable<Product>
    {
        public Product(int productTypeId, int quantity)
        {
            Quantity = quantity;
            Type = new ProductType(productTypeId);
        }

        public Product(ProductType productType, int quantity)
        {
            Quantity = quantity;
            Type = productType;
        }

        public ProductType Type { get; }
        public int Quantity { get; }

        public int CompareTo(Product other)
        {
            return Quantity.CompareTo(other.Quantity);
        }

        public override string ToString()
        {
            return $"{Type}: {Quantity}";
        }
    }

    public class ProductType
    {
        public ProductType(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public override bool Equals(object obj)
        {
            return obj is ProductType type &&
                   Id == type.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class RacksAndProductsData
    {
        public List<Rack> Racks { get; } = new List<Rack>();
        public Dictionary<ProductType, int> ProductsQuantity { get; } = new Dictionary<ProductType, int>();

        public void AddRack(Rack rack)
        {
            Racks.Add(rack);
        }


        public void AddProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
                AddProduct(product);
        }

        private void AddProduct(Product product)
        {
            if (!ProductsQuantity.ContainsKey(product.Type))
                ProductsQuantity.Add(product.Type, 0);

            ProductsQuantity[product.Type] += product.Quantity;
        }
    }

    public class RacksAndProductsParser
    {
        public RacksAndProductsData Parse(string input)
        {
            var inputLines = input.Split('\n');
            var racksCount = ParseRacksCount(inputLines[0]);
            return ParseRacksWithProducts(inputLines, racksCount);
        }

        private static int ParseRacksCount(string inputLine)
        {
            if (int.TryParse(inputLine, out var racksCount))
                return racksCount;

            throw new WrongFormatException(inputLine);
        }

        private static RacksAndProductsData ParseRacksWithProducts(string[] inputLines, int racksCount)
        {
            var data = new RacksAndProductsData();
            var currentLine = 2;
            for (var i = 0; i < racksCount; i++)
            {
                var rack = ParseRack(inputLines, currentLine++);
                var products = ParseProducts(inputLines, currentLine, rack);
                data.AddRack(rack);
                data.AddProducts(products);
                currentLine += rack.Height + 1;
            }

            return data;
        }

        private static IEnumerable<Product> ParseProducts(string[] inputLines, int currentLine, Rack rack)
        {
            var rackProductsLines = inputLines
                .Skip(currentLine)
                .Take(rack.Height);

            return ParseProductsFromLines(rackProductsLines);
        }

        private static Rack ParseRack(IReadOnlyList<string> inputLines, int currentLine)
        {
            return ParseRackFromSizeLine(inputLines[currentLine]);
        }

        private static IEnumerable<Product> ParseProductsFromLines(IEnumerable<string> rackProductsLines)
        {
            foreach (var productId in rackProductsLines
                .SelectMany(productsLine => productsLine.Split()))
                if (int.TryParse(productId, out var intId))
                    yield return new Product(intId, 1);
        }

        private static Rack ParseRackFromSizeLine(string racksSizeLine)
        {
            var strSizes = racksSizeLine?.Split();
            if (strSizes?.Length != 2)
                throw new WrongFormatException(racksSizeLine);

            if (!int.TryParse(strSizes[0], out var height))
                throw new WrongFormatException(strSizes[0]);

            if (!int.TryParse(strSizes[1], out var weight))
                throw new WrongFormatException(strSizes[1]);

            return new Rack(weight, height);
        }
    }

    public class FridgeProductsSorter
    {
        public SortedSet<Rack> SortProductsOnRacks(RacksAndProductsData racksAndProductsData)
        {
            var racksBySize = new SortedSet<Rack>(racksAndProductsData.Racks);
            var productsByQuantity = new SortedSet<Product>(racksAndProductsData.ProductsQuantity
                .Select(x => new Product(x.Key, x.Value)));
            return PlaceProductsInRacks(racksBySize, productsByQuantity);
        }

        private static SortedSet<Rack> PlaceProductsInRacks(SortedSet<Rack> racksBySize,
            SortedSet<Product> productsByQuantity)
        {
            var sortedRacks = new SortedSet<Rack>(new RacksByProductTypeComparer());
            while (productsByQuantity.Count > 0)
            {
                var rack = racksBySize.First();
                var product = productsByQuantity.First();
                PutProductOnRack(rack, product);
                TryAddProductRemainder(rack, product, productsByQuantity);
                productsByQuantity.Remove(product);
                racksBySize.Remove(rack);
                sortedRacks.Add(rack);
            }

            return sortedRacks;
        }

        private static void TryAddProductRemainder(Rack rack, Product product, SortedSet<Product> productsByQuantity)
        {
            var remainderQuantity = product.Quantity - rack.Size;
            if (remainderQuantity > 0)
                productsByQuantity.Add(new Product(product.Type, remainderQuantity));
        }

        private static void PutProductOnRack(Rack rack, Product product)
        {
            var placedQuantity = rack.Size > product.Quantity
                ? product.Quantity
                : rack.Size;
            rack.PutProduct(product.Type, placedQuantity);
        }
    }

    public class FridgeProgram
    {
        private readonly TextWriter writer;

        public FridgeProgram(TextWriter writer)
        {
            this.writer = writer;
        }

        public void RunAlgo(string input)
        {
            var racksAndProductsData = new RacksAndProductsParser()
                .Parse(input);
            var sortedRacks = new FridgeProductsSorter()
                .SortProductsOnRacks(racksAndProductsData);
            PrintRacks(sortedRacks);
        }

        private void PrintRacks(IEnumerable<Rack> racks)
        {
            writer.Write(string.Join("\n", racks));
        }
    }
}