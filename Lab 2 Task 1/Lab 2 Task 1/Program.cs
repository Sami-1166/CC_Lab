using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab_2_Task_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "if (a && b || !c)";
            string pattern = @"(\&\&|\|\||!)";

            MatchCollection matches = Regex.Matches(input, pattern);

            Console.WriteLine("Input String: if (a && b || !c)");
            Console.WriteLine("\nOutput: (Regular expression for logical operators):");
            foreach (Match match in matches)
            {
                Console.WriteLine($"Found logical operator: {match.Value}");
            }
            Console.ReadKey();
        }
    }
}
