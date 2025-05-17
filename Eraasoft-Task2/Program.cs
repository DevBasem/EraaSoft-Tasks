namespace Eraasoft_Task2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int>();

            bool notValid = false;

            do
            {
                Console.WriteLine("P - Print numbers");
                Console.WriteLine("A - Add a number");
                Console.WriteLine("M - Display mean of the numbers");
                Console.WriteLine("S - Display the smallest number");
                Console.WriteLine("L - Display the largest number");
                Console.WriteLine("Q - Quit");
                Console.Write("\n");

                Console.Write("Enter your choice:");
                string choice = Console.ReadLine().ToLower();
            } while (true);
        }
    }
}
