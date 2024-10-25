using System;

class DFA
{
    private enum State
    {
        Start,
        FirstChar,
        SubsequentChar,
        Accept,
        Reject
    }

    public static bool IsValidVariableName(string input)
    {
        State currentState = State.Start;

        foreach (char c in input)
        {
            switch (currentState)
            {
                case State.Start:
                    if (char.IsLetter(c) || c == '_')
                    {
                        currentState = State.FirstChar;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case State.FirstChar:
                    if (char.IsLetterOrDigit(c) || c == '_')
                    {
                        currentState = State.SubsequentChar;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case State.SubsequentChar:
                    if (char.IsLetterOrDigit(c) || c == '_')
                    {
                        currentState = State.Accept;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case State.Accept:
                    if (char.IsLetterOrDigit(c) || c == '_')
                    {
                        // Stay in accept state
                    }
                    else
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }
        }

        return currentState == State.Accept || currentState == State.SubsequentChar;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Enter a variable name to check:");
        string input = Console.ReadLine();

        if (IsValidVariableName(input))
        {
            Console.WriteLine($"{input} is a valid C variable name.");
        }
        else
        {
            Console.WriteLine($"{input} is not a valid C variable name.");
        }
    }
}
