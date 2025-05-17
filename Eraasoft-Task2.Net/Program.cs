namespace Eraasoft_Task2.Net
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int>();

            bool isRunning = true;

            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("---------------------------------");
                Console.WriteLine("           MAIN MENU            ");
                Console.WriteLine("---------------------------------");
                Console.ResetColor();

                Console.WriteLine("P - Print numbers");
                Console.WriteLine("A - Add a number");
                Console.WriteLine("M - Display mean of the numbers");
                Console.WriteLine("S - Display the smallest number");
                Console.WriteLine("L - Display the largest number");
                Console.WriteLine("V - Search for a number in the list");
                Console.WriteLine("I - Sort in ascending order");
                Console.WriteLine("O - Sort in descending order");
                Console.WriteLine("X - Swap two numbers in the list");
                Console.WriteLine("C - Clear the current list");
                Console.WriteLine("Q - Quit");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("---------------------------------");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Enter your choice: ");
                Console.ResetColor();

                string input = Console.ReadLine();
                string choice = input.ToLower();

                switch (choice)
                {
                    case "p":
                        if (numbers.Count == 0)
                        {
                            Console.WriteLine("[] - the list is empty\n");
                        }
                        else
                        {
                            Console.Write("[ ");
                            for (int i = 0; i < numbers.Count; i++)
                            {
                                Console.Write($"{numbers[i]} ");
                            }
                            Console.WriteLine("]\n");
                        }
                        break;
                    case "a":
                        Console.Write("Enter a number to add: ");
                        int number = Convert.ToInt32(Console.ReadLine());
                        bool numberExists = false;

                        for(int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] == number)
                            {
                                numberExists = true;
                                break;
                            }
                        }
                        if (numberExists)
                        {
                            Console.WriteLine($"{number} already exists in the list\n");
                            break;
                        }
                        else
                        {
                            numbers.Add(number);
                        }
                        Console.WriteLine($"{number} added\n");
                        break;
                    case "m":
                        if (numbers.Count == 0)
                        {
                            Console.WriteLine("Unable to calculate the mean - no data\n");
                        }
                        else
                        {
                            double mean = 0;
                            double sum = 0;

                            for (int i = 0; i < numbers.Count; i++)
                            {
                                sum += numbers[i];
                            }

                            mean = sum / numbers.Count;
                            Console.WriteLine($"Mean: {mean}\n");
                        }
                        break;
                    case "s":
                        if (numbers.Count == 0)
                        {
                            Console.WriteLine("Unable to determine the smallest number - list is empty\n");
                        }
                        else
                        {
                            int smallest = numbers[0];
                            for (int i = 1; i < numbers.Count; i++)
                            {
                                if (numbers[i] < smallest)
                                {
                                    smallest = numbers[i];
                                }
                            }
                            Console.WriteLine($"The smallest number is {smallest}\n");
                        }
                        break;
                    case "l":
                        if (numbers.Count == 0)
                        {
                            Console.WriteLine("Unable to determine the largest number - list is empty\n");
                        }
                        else
                        {
                            int largest = numbers[0];
                            for (int i = 1; i < numbers.Count; i++)
                            {
                                if (numbers[i] > largest)
                                {
                                    largest = numbers[i];
                                }
                            }
                            Console.WriteLine($"The largest number is {largest}\n");
                        }
                        break;
                    case "v":
                        Console.Write("Enter a number to search for: ");
                        int searchNumber = Convert.ToInt32(Console.ReadLine());
                        int searchNumberIndex = -1;

                        for (int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] == searchNumber)
                            {
                                searchNumberIndex = i;
                                break;
                            }
                        }

                        if (searchNumberIndex == -1)
                        {
                            Console.WriteLine($"{searchNumber} not found in the list\n");
                        }
                        else
                        {
                            Console.WriteLine($"The number index is {searchNumberIndex}\n");
                        }
                        break;
                    case "c":
                        numbers.Clear();
                        Console.WriteLine("The list has been cleared\n");
                        break;                    
                    case "i":
                        for (int i = 0; i < numbers.Count - 1; i++)
                        {
                            for (int j = 0; j < numbers.Count - i - 1; j++)
                            {
                                if (numbers[j] > numbers[j + 1])
                                {
                                    int temp = numbers[j];
                                    numbers[j] = numbers[j + 1];
                                    numbers[j + 1] = temp;
                                }
                            }
                        }
                        Console.WriteLine("The list has been sorted in ascending order\n");
                        break;                    
                    case "o":
                        for (int i = 0; i < numbers.Count - 1; i++)
                        {
                            for (int j = 0; j < numbers.Count - i - 1; j++)
                            {
                                if (numbers[j] < numbers[j + 1])
                                {
                                    int temp = numbers[j];
                                    numbers[j] = numbers[j + 1];
                                    numbers[j + 1] = temp;
                                }
                            }
                        }
                        Console.WriteLine("The list has been sorted in descending order\n");
                        break;                    
                    case "x":
                        Console.Write("Enter the first number: ");
                        int firstNumber = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Enter the second number: ");
                        int secondNumber = Convert.ToInt32(Console.ReadLine());

                        int firstNumberIndex = -1;
                        int secondNumberIndex = -1;

                        for (int i = 0; i < numbers.Count; i++)
                        {
                            if (numbers[i] == firstNumber)
                            {
                                firstNumberIndex = i;
                            }
                            if (numbers[i] == secondNumber)
                            {
                                secondNumberIndex = i;
                            }
                        }

                        if (firstNumberIndex == -1)
                        {
                            Console.WriteLine($"{firstNumber} not found in the list\n");
                        }
                        else if (secondNumberIndex == -1)
                        {
                            Console.WriteLine($"{secondNumber} not found in the list\n");
                        }
                        else
                        {
                            numbers[firstNumberIndex] = secondNumber;
                            numbers[secondNumberIndex] = firstNumber;
                            Console.WriteLine($"The numbers {firstNumber} and {secondNumber} have been swapped\n");
                        }
                        break;
                    case "q":
                        isRunning = false;
                        Console.WriteLine("Goodbye");
                        break;
                    default:
                        Console.WriteLine("Unknown selection, please try again\n");
                        break;
                }
            } while (isRunning);
        }
    }
}
