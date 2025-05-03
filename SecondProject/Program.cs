namespace SecondProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your favorite number between 1 and 100: ");
            int userFavNumber = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"No really!!, {userFavNumber} is my favorite number too!");
        }
    }
}
