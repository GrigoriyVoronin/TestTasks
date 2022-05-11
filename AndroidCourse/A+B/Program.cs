using System;
using System.Linq;

namespace A_B
{
    class Program
    {
        static void Main()
        {
            var arr = Console.ReadLine()?
                .Split()
                .Select(int.Parse)
                .ToArray();
            if (arr != null && arr.Length == 2)
                Console.WriteLine(arr[0] + arr[1]);
            else
                throw new ArgumentException();
        }
    }
}
