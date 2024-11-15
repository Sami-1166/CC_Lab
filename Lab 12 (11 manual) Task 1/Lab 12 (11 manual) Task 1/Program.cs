using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SemanticAnalyzer
{
    class Program
    {
        // Example symbol table: List of lists to simulate rows in a table
        static List<List<string>> Symboltable = new List<List<string>>()
        {
            new List<string> { "int", "a", "int", "5" },
            new List<string> { "int", "b", "int", "10" },
            new List<string> { "int", "c", "int", "0" }
        };

        // Constants list for constant folding
        static List<int> Constants = new List<int>();

        // Example tokenized input to parse
        static List<string> finalArray = new List<string> { "int", "c", "=", "a", "+", "b", ";" };

        // Regex to match variables (for example, "a", "b", "c")
        static Regex variable_Reg = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");

        static void Main(string[] args)
        {
            // Parse the tokenized input
            for (int i = 0; i < finalArray.Count; i++)
            {
                if (finalArray[i] == "+" || finalArray[i] == "-" || finalArray[i] == ">")
                {
                    Semantic_Analysis(i);
                }
            }

            // Display updated symbol table
            Console.WriteLine("Updated Symbol Table:");
            foreach (var row in Symboltable)
            {
                Console.WriteLine(string.Join(" | ", row));
            }

            // Display constants added during constant folding
            Console.WriteLine("\nConstants Folded:");
            foreach (var constant in Constants)
            {
                Console.WriteLine(constant);
            }
            Console.ReadKey();
        }

        static void Semantic_Analysis(int k)
        {
            if (finalArray[k].Equals("+") || finalArray[k].Equals("-"))
            {
                if (variable_Reg.IsMatch(finalArray[k - 1]) && variable_Reg.IsMatch(finalArray[k + 1]))
                {
                    string type = finalArray[k - 3]; // type is expected before the variable
                    string left_side = finalArray[k - 2];
                    string before = finalArray[k - 1];
                    string after = finalArray[k + 1];

                    int left_side_i = GetSymbolTableIndex(left_side);
                    int before_i = GetSymbolTableIndex(before);
                    int after_i = GetSymbolTableIndex(after);

                    if (left_side_i != -1 && before_i != -1 && after_i != -1 &&
                        type == Symboltable[before_i][2] && type == Symboltable[after_i][2])
                    {
                        int Ans;
                        if (finalArray[k] == "+")
                        {
                            Ans = int.Parse(Symboltable[before_i][3]) + int.Parse(Symboltable[after_i][3]);
                        }
                        else // "-"
                        {
                            Ans = int.Parse(Symboltable[before_i][3]) - int.Parse(Symboltable[after_i][3]);
                        }

                        Constants.Add(Ans);
                        Symboltable[left_side_i][3] = Ans.ToString();
                        Console.WriteLine($"Updated {left_side} to {Ans}");
                    }
                }
            }
            else if (finalArray[k].Equals(">"))
            {
                if (variable_Reg.IsMatch(finalArray[k - 1]) && variable_Reg.IsMatch(finalArray[k + 1]))
                {
                    string before = finalArray[k - 1];
                    string after = finalArray[k + 1];

                    int before_i = GetSymbolTableIndex(before);
                    int after_i = GetSymbolTableIndex(after);

                    if (before_i != -1 && after_i != -1)
                    {
                        int beforeValue = int.Parse(Symboltable[before_i][3]);
                        int afterValue = int.Parse(Symboltable[after_i][3]);

                        if (beforeValue > afterValue)
                        {
                            Console.WriteLine($"{before} > {after} is true. Executing 'if' block.");
                        }
                        else
                        {
                            Console.WriteLine($"{before} > {after} is false. Skipping 'if' block.");
                        }
                    }
                }
            }
        }

        // Helper function to get index in Symbol Table for a given variable
        static int GetSymbolTableIndex(string variable)
        {
            for (int i = 0; i < Symboltable.Count; i++)
            {
                if (Symboltable[i][1] == variable)
                    return i;
            }
            return -1; // Variable not found
        }
    }
}