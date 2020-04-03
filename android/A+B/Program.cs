using System;
using System.Linq;

namespace A_B
{
    class Program
    {
        static void Main(string[] args)
        {
            var arr = Console.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
            Console.WriteLine(arr[0] + arr[1]);
        }
    }
}
