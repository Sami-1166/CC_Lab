using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab_2_Task_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "if (a == b || x != y && p > q && r <= s)";
            string pattern = @"(\=\=|\!\=|>=|<=|>|<)";

            MatchCollection matches = Regex.Matches(input, pattern);

            Console.WriteLine("Input String: if (a == b || x != y && p > q && r <= s)");
            Console.WriteLine("\nOutput: (Regular expression for relational operators):");
            foreach (Match match in matches)
            {
                Console.WriteLine($"Found relational operator: {match.Value}");
            }
            Console.ReadKey();
        }
    }
}
