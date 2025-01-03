using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter usernames (separated by commas): ");
        string input = Console.ReadLine();
        string[] usernames = input.Split(',').Select(u => u.Trim()).ToArray();

        List<string> validUsernames = new List<string>();
        List<string> invalidUsernames = new List<string>();

        Dictionary<string, string> passwordStrengths = new Dictionary<string, string>();
        Dictionary<string, string> passwords = new Dictionary<string, string>();

        foreach (string username in usernames)
        {
            if (ValidateUsername(username, out string errorMessage))
            {
                validUsernames.Add(username);
                AnalyzeUsername(username);

                string password = GeneratePassword();
                passwords[username] = password;
                passwordStrengths[username] = EvaluatePasswordStrength(password);
            }
            else
            {
                Console.WriteLine($"{username} - Invalid ({errorMessage})");
                invalidUsernames.Add(username);
            }
        }

        Console.WriteLine("\nSummary:");
        Console.WriteLine($"- Total Usernames: {usernames.Length}");
        Console.WriteLine($"- Valid Usernames: {validUsernames.Count}");
        Console.WriteLine($"- Invalid Usernames: {invalidUsernames.Count}\n");

        if (validUsernames.Count > 0)
        {
            foreach (string username in validUsernames)
            {
                Console.WriteLine($"{username} - Valid");
                Console.WriteLine($"Generated Password: {passwords[username]} (Strength: {passwordStrengths[username]})\n");
            }
        }

        SaveResultsToFile(usernames.Length, validUsernames.Count, invalidUsernames.Count, validUsernames, passwords, passwordStrengths);

        if (invalidUsernames.Count > 0)
        {
            Console.Write("Do you want to retry invalid usernames? (y/n): ");
            string retry = Console.ReadLine().Trim().ToLower();

            if (retry == "y")
            {
                Console.Write("Enter invalid usernames: ");
                string retryInput = Console.ReadLine();
                string[] retryUsernames = retryInput.Split(',').Select(u => u.Trim()).ToArray();

                foreach (string retryUsername in retryUsernames)
                {
                    if (ValidateUsername(retryUsername, out string retryErrorMessage))
                    {
                        Console.WriteLine($"{retryUsername} - Valid");
                        AnalyzeUsername(retryUsername);

                        string password = GeneratePassword();
                        Console.WriteLine($"Generated Password: {password} (Strength: {EvaluatePasswordStrength(password)})\n");
                    }
                    else
                    {
                        Console.WriteLine($"{retryUsername} - Invalid ({retryErrorMessage})");
                    }
                }
            }
        }

        Console.WriteLine("Processing complete.");
        Console.ReadKey();
    }

    static bool ValidateUsername(string username, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!Regex.IsMatch(username, "^[a-zA-Z]"))
        {
            errorMessage = "Username must start with a letter.";
            return false;
        }

        if (!Regex.IsMatch(username, "^[a-zA-Z0-9_]+$"))
        {
            errorMessage = "Username can only contain letters, numbers, and underscores.";
            return false;
        }

        if (username.Length < 5 || username.Length > 15)
        {
            errorMessage = "Username length must be between 5 and 15.";
            return false;
        }

        return true;
    }

    static void AnalyzeUsername(string username)
    {
        int uppercase = username.Count(char.IsUpper);
        int lowercase = username.Count(char.IsLower);
        int digits = username.Count(char.IsDigit);
        int underscores = username.Count(c => c == '_');

        Console.WriteLine($"Letters: {uppercase + lowercase} (Uppercase: {uppercase}, Lowercase: {lowercase}), Digits: {digits}, Underscores: {underscores}");
    }

    static string GeneratePassword()
    {
        Random random = new Random();
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";

        string allChars = uppercase + lowercase + digits + special;

        char[] password = new char[12];

        for (int i = 0; i < 2; i++) password[i] = uppercase[random.Next(uppercase.Length)];
        for (int i = 2; i < 4; i++) password[i] = lowercase[random.Next(lowercase.Length)];
        for (int i = 4; i < 6; i++) password[i] = digits[random.Next(digits.Length)];
        for (int i = 6; i < 8; i++) password[i] = special[random.Next(special.Length)];
        for (int i = 8; i < 12; i++) password[i] = allChars[random.Next(allChars.Length)];

        return new string(password.OrderBy(_ => random.Next()).ToArray());
    }

    static string EvaluatePasswordStrength(string password)
    {
        int length = password.Length;
        int uppercase = password.Count(char.IsUpper);
        int lowercase = password.Count(char.IsLower);
        int digits = password.Count(char.IsDigit);
        int special = password.Count(c => "!@#$%^&*".Contains(c));

        if (length >= 12 && uppercase >= 2 && lowercase >= 2 && digits >= 2 && special >= 2)
            return "Strong";

        if (length >= 8 && uppercase >= 1 && lowercase >= 1 && digits >= 1 && special >= 1)
            return "Medium";

        return "Weak";
    }

    static void SaveResultsToFile(int total, int valid, int invalid, List<string> validUsernames, Dictionary<string, string> passwords, Dictionary<string, string> strengths)
    {
        using (StreamWriter writer = new StreamWriter("UserDetails.txt"))
        {
            writer.WriteLine("Validation Results:");

            foreach (string username in validUsernames)
            {
                writer.WriteLine($"{username} - Valid");
                writer.WriteLine($"Generated Password: {passwords[username]} (Strength: {strengths[username]})\n");
            }

            writer.WriteLine("Summary:");
            writer.WriteLine($"- Total Usernames: {total}");
            writer.WriteLine($"- Valid Usernames: {valid}");
            writer.WriteLine($"- Invalid Usernames: {invalid}");
        }
    }
}
