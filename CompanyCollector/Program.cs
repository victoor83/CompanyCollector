using System;
using System.Collections.Generic;
using System.IO;

namespace CompanyCollector
{
    public delegate void DelLogger(string message);
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Companies companies = new Companies(LogIn);
            var list = companies.GetCompanies(1000);

            File.WriteAllLines(@"d:\companies.txt", list);
        }

        static void LogIn(string log)
        {
            Console.WriteLine(log);
        }


    }
}

