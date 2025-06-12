namespace Eraasoft_Task4_Search.Net
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Enter integers separated by space:");
                string numbersInput = Console.ReadLine();
                CheckForDuplicateNumbers(numbersInput);
                Console.WriteLine("All numbers are unique.");

                Console.WriteLine();

                Console.WriteLine("Enter a string:");
                string textInput = Console.ReadLine();
                CheckForVowels(textInput);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        static void CheckForDuplicateNumbers(string input)
        {
            string[] parts = input.Split(' ');
            int length = parts.Length;
            int[] numbers = new int[length];

            for (int i = 0; i < length; i++)
            {
                numbers[i] = int.Parse(parts[i]);
            }

            for (int i = 0; i < length; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    if (numbers[i] == numbers[j])
                    {
                        throw new Exception("Duplicate number found: " + numbers[i]);
                    }
                }
            }
        }

        static void CheckForVowels(string input)
        {
            char[] vowels = { 'a', 'e', 'i', 'o', 'u',
                              'A', 'E', 'I', 'O', 'U' };

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < vowels.Length; j++)
                {
                    if (input[i] == vowels[j])
                    {
                        Console.WriteLine("The string contains vowels.");
                        return;
                    }
                }
            }

            throw new Exception("The string does not contain any vowels.");
        }
    }
}