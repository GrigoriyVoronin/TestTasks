using System;
using System.Linq;
using System.Collections.Generic;
using SberFight2022;

class Program
{
    public static void Main(string[] args)
    {
        //Console.WriteLine(Task8.GetResult(new List<string>()
        //{
        //    "0-0-0-0",
        //    "1-1-1-0",
        //    "0-0-1-0",
        //    "0-0-1-0"
        //}));       
        ////92
        Console.WriteLine(Task8.GetResult(new List<string>()
        {
            "0-0-1-0-0-0-0-0",
            "0-0-1-0-1-1-1-0",
            "0-0-1-1-1-0-1-0",
            "0-0-0-0-0-0-1-0",
            "0-0-0-0-0-0-1-0",
            "0-0-0-0-1-1-1-0",
            "0-0-0-0-1-0-0-0"
        }));
        //268
    }
}