using System;
using System.Collections.Generic;

public class SLRParser
{
    // Terminal symbols (tokens)
    private static readonly HashSet<string> terminals = new HashSet<string>
    {
        "begin", "end", "int", "float", "if", "for", "else", "then", "print", "id", "=", "+", "(", ")", ";", "number"
    };

    // Non-terminal symbols
    private static readonly HashSet<string> nonTerminals = new HashSet<string>
    {
        "S", "L", "stmt", "var_decl", "assign_stmt", "if_stmt", "print_stmt", "expr", "term", "constant", "number", "id"
    };

    // Action table (for shift and reduce actions)
    private static readonly Dictionary<Tuple<int, string>, string> actionTable = new Dictionary<Tuple<int, string>, string>
    {
        // State 0 (Initial state)
        { new Tuple<int, string>(0, "begin"), "S2" },
        { new Tuple<int, string>(0, "int"), "S3" },
        { new Tuple<int, string>(0, "float"), "S4" },
        { new Tuple<int, string>(0, "id"), "S5" },

        // State 2 (After beginning block)
        { new Tuple<int, string>(2, "int"), "S6" },  // Start processing variable declarations
        { new Tuple<int, string>(2, "float"), "S7" }, // Start processing variable declarations
        { new Tuple<int, string>(2, "id"), "S8" },    // Start processing variable declarations

        // State 3 (Processing 'int' declaration)
        { new Tuple<int, string>(3, "id"), "S9" },    // Expect an id after 'int'
        { new Tuple<int, string>(3, "="), "S10" },    // Optional assignment after variable declaration
        { new Tuple<int, string>(3, ";"), "S11" },    // End of declaration

        // State 4 (Processing 'float' declaration)
        { new Tuple<int, string>(4, "id"), "S12" },    // Expect an id after 'float'
        { new Tuple<int, string>(4, "="), "S13" },    // Optional assignment after variable declaration
        { new Tuple<int, string>(4, ";"), "S14" },    // End of declaration

        // After processing 'int id'
        { new Tuple<int, string>(6, "="), "S15" },    // Expect assignment
        { new Tuple<int, string>(6, "id"), "S16" },   // Expect an id (for assignment or expression)
        { new Tuple<int, string>(6, "number"), "S17" },// Expect a number (for assignment)

        // Handling 'if' statement
        { new Tuple<int, string>(7, "id"), "S24" },
        { new Tuple<int, string>(7, "number"), "S25" },

        // After parsing 'id', we expect an expression or assignment
        { new Tuple<int, string>(9, "="), "S18" },    // Assignment after 'id'
        { new Tuple<int, string>(9, ";"), "S19" },    // End of declaration
        { new Tuple<int, string>(9, "id"), "S20" },   // Expect 'id' or other assignments

        // Expression handling
        { new Tuple<int, string>(16, "="), "S21" },   // Expecting assignment (after id)
        { new Tuple<int, string>(16, "+"), "S22" },   // Continue expression (after id)
        { new Tuple<int, string>(16, ";"), "S23" },   // End of statement or expression
        { new Tuple<int, string>(16, "number"), "S17" }, // Expect number for assignment or expression
        { new Tuple<int, string>(16, "id"), "S20" },  // Expect id for continuation of expression

        { new Tuple<int, string>(17, ";"), "S23" },    // End of expression or statement
        { new Tuple<int, string>(17, "int"), "S6" },    // New variable declaration
        { new Tuple<int, string>(17, "id"), "S16" },    // Start new assignment or expression
        { new Tuple<int, string>(17, "number"), "S17" }, // Continue parsing expressions if necessary

        { new Tuple<int, string>(20, "="), "S18" },   // Assignment operation
        { new Tuple<int, string>(20, "+"), "S22" },   // Continue expression
        { new Tuple<int, string>(20, ";"), "S19" },   // End of assignment
        { new Tuple<int, string>(20, "id"), "S20" },    // Continue assignment or expression
        { new Tuple<int, string>(20, "if"), "S21" },    // Move to state that handles 'if' condition

        { new Tuple<int, string>(21, "id"), "S22" },    // After encountering 'if', expect a condition or expression

        { new Tuple<int, string>(22, "then"), "S23" },   // After condition (id), expect 'then'
        { new Tuple<int, string>(22, "print"), "S24" },   // After condition, expect a statement, like 'print'


        // After 'print', expect either 'else' or 'end'
        { new Tuple<int, string>(24, "else"), "S25" },
        { new Tuple<int, string>(24, "end"), "S26" },


        // End of parsing
        { new Tuple<int, string>(99, "end"), "A" }
    };

    // Goto table (state transitions after reductions)
    private static readonly Dictionary<Tuple<int, string>, int> gotoTable = new Dictionary<Tuple<int, string>, int>
    {
        { new Tuple<int, string>(0, "L"), 1 },   // After 'begin', move to L (Statement)
        { new Tuple<int, string>(1, "stmt"), 2 }, // Parse a statement
        { new Tuple<int, string>(2, "var_decl"), 3 }, // Parse a variable declaration
        { new Tuple<int, string>(3, "assign_stmt"), 4 }, // Parse an assignment
        { new Tuple<int, string>(4, "expr"), 5 },  // Parse an expression
        { new Tuple<int, string>(5, "if_stmt"), 6 }, // Parse an 'if' statement
        { new Tuple<int, string>(6, "print_stmt"), 7 }, // Parse a 'print' statement
    };

    // Method to tokenize the input string
    public static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        string[] parts = input.Split(new char[] { ' ', '(', ')', '=', ';', '{', '}', '+', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (terminals.Contains(part))
                tokens.Add(part);
            else if (int.TryParse(part, out _))
                tokens.Add("number"); // Treat digits as "number"
            else
                tokens.Add("id"); // Treat others as "id"
        }
        return tokens;
    }

    // Parsing method using the SLR parsing technique
    public static bool Parse(List<string> tokens)
    {
        var stack = new Stack<int>();  // Stack of states
        var input = new Queue<string>(tokens);  // Queue of tokens
        stack.Push(0);  // Push the initial state

        while (input.Count > 0)
        {
            int state = stack.Peek();
            string symbol = input.Peek();

            var actionKey = new Tuple<int, string>(state, symbol);

            // Check if we need to shift or reduce
            if (actionTable.ContainsKey(actionKey))
            {
                string action = actionTable[actionKey];

                if (action.StartsWith("S")) // Shift operation
                {
                    int nextState = int.Parse(action.Substring(1));
                    stack.Push(nextState);
                    input.Dequeue();  // Consume the token
                    Console.WriteLine($"Shift: {symbol} (State {state} -> State {nextState})");
                }
                else if (action == "A") // Accept operation
                {
                    Console.WriteLine("Accept");
                    return true;
                }
                else
                {
                    // Reduction (for simplicity, reduce with the first production rule)
                    Console.WriteLine($"Reduce: {action}");
                    stack.Pop();  // Pop one or more states based on the production rule

                    // After reduction, we transition using the goto table
                    string topSymbol = "stmt"; // You can replace this with the actual non-terminal being reduced
                    if (gotoTable.ContainsKey(new Tuple<int, string>(stack.Peek(), topSymbol)))
                    {
                        stack.Push(gotoTable[new Tuple<int, string>(stack.Peek(), topSymbol)]);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: Unexpected symbol '{symbol}' at state {state}");
                return false;
            }
        }

        return false;
    }

    public static void Main(string[] args)
    {
        string input = "begin int a=5; int b=10; int c=0; c=a+b; if(c>a) print a; else print c; end";
        var tokens = Tokenize(input);
        Console.WriteLine("Input Tokens: ");
        foreach (var token in tokens)
        {
            Console.Write(token + " ");
        }

        Console.WriteLine("\nParsing...");
        bool result = Parse(tokens);
        if (result)
        {
            Console.WriteLine("Input parsed successfully!");
        }
        else
        {
            Console.WriteLine("Parsing failed.");
        }
        Console.ReadKey();
    }
}
