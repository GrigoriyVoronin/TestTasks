using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BFTIndex.Models;

namespace BFTIndex
{
    public static class Program
    {
        private static readonly Dictionary<char, char> NormalizationTable = new Dictionary<char, char>
        {
            ['ё'] = 'е',
            ['й'] = 'и',
            ['щ'] = 'ш',
            ['ъ'] = 'ь',
        };

        private static readonly string[] Stopwords =
        {
            "ибо",
            "и",
            "но",
            "а",
            "или"
        };

        public static void Main()
        {
            var documents = Directory.EnumerateFiles("Documents")
                .Select(path => new FileInfo(path))
                .ToDictionary(file => file.Name.Replace(file.Extension, ""), file => File.ReadAllText(file.FullName));

            documents = new Dictionary<string, string>
            {
                {"1", "aa bb cc"},
                {"2", "aa aa aa aa aa aa bb cc bb"},
                {"3", "aa bb cc ww"},
                {"4", "bb bb bb cc bb"},
                {"5", "aa aa bb cc bb"},
                {"6" ,"aa aa aa aa aa aa aa aa bb aa"},
                {"7" ,"aa bb aa aa aa aa aa aa bb cc"},
                {"8", "bb xx yy zz"},
                {"9",  "bb xx yy zz ww"},
                {"10", "aa xx yy zz ww"}
            };
            var index = new FullTextIndexFactory().Create(Stopwords, NormalizationTable);

            foreach (var document in documents)
                index.AddOrUpdate(document.Key, document.Value);

            while (true)
            {
                var query = ReadQuery();
                if (query.ToLowerInvariant().Trim() == "q")
                    return;

                var sw = Stopwatch.StartNew();
                const int times = 1;
                MatchedDocument[] result = index.Search(query);


                Console.WriteLine($" Found {result.Length} documents, elapsed {sw.Elapsed.TotalSeconds / times}");
                foreach (var document in result.OrderByDescending(d => d.Weight))
                    Console.WriteLine($"  [{document.Weight:E3}]\t{document.Id}");
                Console.WriteLine();
            }
        }

        private static string ReadQuery()
        {
            using (new ConsoleColors(ConsoleColor.Green))
                Console.Write("query (q for exit)");

            Console.Write("$ ");

            using (new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkBlue))
                return Console.ReadLine();
        }

        private class ConsoleColors : IDisposable
        {
            private readonly ConsoleColor oldForeground;
            private readonly ConsoleColor oldBackground;

            public ConsoleColors(ConsoleColor? foreground = null, ConsoleColor? background = null)
            {
                oldForeground = Console.ForegroundColor;
                oldBackground = Console.BackgroundColor;

                Console.ForegroundColor = foreground ?? oldForeground;
                Console.BackgroundColor = background ?? oldBackground;
            }

            public void Dispose()
            {
                Console.ForegroundColor = oldForeground;
                Console.BackgroundColor = oldBackground;
            }
        }
    }
}
