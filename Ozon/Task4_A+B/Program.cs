using System;
using System.Linq;
using System.Numerics;

namespace Task_4_A_B
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var numbers = Console.ReadLine().Split().Select(x => BigInteger.Parse(x)).ToArray();
            Console.WriteLine((numbers[0] + numbers[1]).ToString());
        }
    }
}