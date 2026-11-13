namespace BattleSystem.UI
{
    // Validated console input. Loops until the user enters acceptable data.
    public static class InputReader
    {
        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int value))
                {
                    Console.WriteLine("Invalid input. Please enter a whole number.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Console.WriteLine($"Please enter a number between {min} and {max}.");
                    continue;
                }

                return value;
            }
        }

        public static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input)) return input.Trim();

                Console.WriteLine("Input cannot be empty.");
            }
        }

        public static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " (y/n): ");
                string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (input == "y" || input == "yes") return true;
                if (input == "n" || input == "no") return false;

                Console.WriteLine("Please enter y or n.");
            }
        }
    }
}
