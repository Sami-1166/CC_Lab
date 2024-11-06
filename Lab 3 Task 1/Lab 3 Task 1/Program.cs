using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab_3_Task_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string pattern = @"^-?\d{1,6}(\.\d{1,5})?$";
            string[] inputs = { "123", "-12", "1.23", "-1.2", "12345.6", "1234567", "12.3456", "123.4567" };

            foreach (string input in inputs)
            {
                if (Regex.IsMatch(input, pattern))
                {
                    Console.WriteLine($"{input} is a valid floating point number with a max length of 6.");
                }
                else
                {
                    Console.WriteLine($"{input} is not valid.");
                }
            }
            Console.ReadKey();
        }
    }
}
