using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Task1_UniqueNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Stopwatch();
            t.Start();
            var input = File.ReadAllLines("input-201.txt").Select(x=>long.Parse(x)).ToArray();
            var numbers = new HashSet<long>();
            foreach (var str in input)
                if (numbers.Contains(str))
                    numbers.Remove(str);
                else
                    numbers.Add(str);
            t.Stop();
            File.WriteAllLines("input-201.a.txt", new[] {numbers.First().ToString(), t.Elapsed.Ticks.ToString()});
        }
    }
}
