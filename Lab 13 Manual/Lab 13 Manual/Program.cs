using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    // Symbol table to store variable declarations
    static Dictionary<string, string> symbolTable = new Dictionary<string, string>();

    static void Main(string[] args)
    {
        string[] code = {
            "int x;",
            "float y;",
            "int x;", // Multiple declaration error
            "float y;", // Multiple declaration error
            "int z;",
            "jjj = 10;", // No declaration error
            "b = 20;", // No declaration error
            "float a = 1.5;",
            "x = (int)a;", // Casting error: float to int
        };

        AnalyzeCode(code);
        Console.ReadKey();
    }

    static void AnalyzeCode(string[] code)
    {
        foreach (var line in code)
        {
            LexicalAndParse(line);
        }
    }

    static void LexicalAndParse(string line)
    {
        // Use regular expression to split the line into tokens
        string[] tokens = Regex.Split(line, @"\s+|(?=[-+*/=();])|(?<=[-+*/=();])");

        tokens = Array.FindAll(tokens, t => t != "");

        if (tokens.Length == 0) return;

        // Declaration
        if (tokens[0] == "int" || tokens[0] == "float")
        {
            string type = tokens[0];
            string variable = tokens[1];

            if (symbolTable.ContainsKey(variable))
            {
                Console.WriteLine($"Declaration Error: MULTIPLE_DECLARATION, variable ({variable})");
            }
            else
            {
                symbolTable[variable] = type;
            }
        }
        // Assignment or Casting
        else
        {
            string variable = tokens[0];
            if (!symbolTable.ContainsKey(variable))
            {
                Console.WriteLine($"Declaration Error: NO_DECLARATION, variable ({variable})");
                return;
            }

            if (tokens.Length > 2 && tokens[1] == "=")
            {
                string assignedValue = tokens[2];

                // Check for casting (e.g., "(int)a")
                if (assignedValue.StartsWith("(") && assignedValue.EndsWith(")"))
                {
                    string castType = assignedValue.Substring(1, assignedValue.Length - 2).Split(' ')[0];
                    string castVariable = assignedValue.Substring(assignedValue.IndexOf(' ') + 1, assignedValue.Length - assignedValue.IndexOf(' ') - 2);

                    // Ensure castVariable exists and is of type float
                    if (symbolTable.ContainsKey(castVariable) && symbolTable[castVariable] == "float")
                    {
                        // Ensure the target variable is of type int
                        if (symbolTable[variable] == "int" && castType == "int")
                        {
                            Console.WriteLine($"Casting Error: FLOAT_INT_CASTING, variable ({variable})");
                        }
                    }
                }
            }
        }
    }
}