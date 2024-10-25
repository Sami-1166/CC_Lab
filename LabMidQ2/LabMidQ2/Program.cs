using System;
using System.Text;
using System.Text.RegularExpressions;

class PasswordGenerator
{
    static void Main(string[] args)
    {
        string registrationNumberLast2Digits = "41"; 
        char firstNameSecondLetter = 'a';
        char lastNameSecondLetter = 'l'; 
        string favoriteMovieChars = "lo";
        int requiredLength = 14;
        string specialChars = "!$%&()*+,-./:;<=>?@[\\]^_`{|}~";

        string password = GeneratePassword(registrationNumberLast2Digits, firstNameSecondLetter, lastNameSecondLetter, favoriteMovieChars, specialChars, requiredLength);

        string pattern = $@"^(?!.*#).{{{requiredLength}}}$";
        Regex regex = new Regex(pattern);

        if (regex.IsMatch(password))
        {
            Console.WriteLine("Generated Password: " + password);
        }
        else
        {
            Console.WriteLine("Password generation failed.");
        }
        Console.ReadKey();
    }

    static string GeneratePassword(string regDigits, char firstLetter, char lastLetter, string movieChars, string specialChars, int length)
    {
        Random random = new Random();
        StringBuilder passwordBuilder = new StringBuilder();

        passwordBuilder.Append(regDigits);
        passwordBuilder.Append(firstLetter);
        passwordBuilder.Append(lastLetter);
        passwordBuilder.Append(movieChars);

        for (int i = 0; i < 5; i++)
        {
            passwordBuilder.Append(specialChars[random.Next(specialChars.Length)]);
        }

        while (passwordBuilder.Length < length)
        {
            passwordBuilder.Append((char)random.Next(33, 127));
        }

        char[] passwordArray = passwordBuilder.ToString().ToCharArray();
        for (int i = 0; i < passwordArray.Length; i++)
        {
            int j = random.Next(passwordArray.Length);
            char temp = passwordArray[i];
            passwordArray[i] = passwordArray[j];
            passwordArray[j] = temp;
        }

        return new string(passwordArray);
    }
}
