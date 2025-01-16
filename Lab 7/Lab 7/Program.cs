using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1_Task_1
{
    internal class ScientificCalculator
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Scientific Calculator");
                Console.WriteLine("Choose an operation (Enter its number): ");
                Console.WriteLine("1. Sin");
                Console.WriteLine("2. Cos");
                Console.WriteLine("3. Tan");
                Console.WriteLine("4. Log");
                Console.WriteLine("5. Exit");

                int choice = int.Parse(Console.ReadLine());

                if (choice == 5)
                {
                    break;
                }

                Console.WriteLine("Enter the value (in degrees for trigonometric functions):");
                double value = double.Parse(Console.ReadLine());

                double result = 0;

                switch (choice)
                {
                    case 1:
                        result = Math.Sin(DegreeToRadian(value));
                        Console.WriteLine($"sin({value}) = {result}");
                        break;
                    case 2:
                        result = Math.Cos(DegreeToRadian(value));
                        Console.WriteLine($"cos({value}) = {result}");
                        break;
                    case 3:
                        result = Math.Tan(DegreeToRadian(value));
                        Console.WriteLine($"tan({value}) = {result}");
                        break;
                    case 4:
                        if (value > 0)
                        {
                            result = Math.Log(value);
                            Console.WriteLine($"log({value}) = {result}");
                        }
                        else
                        {
                            Console.WriteLine("Logarithm of non-positive numbers is undefined.");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
            }

        }
        static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}
