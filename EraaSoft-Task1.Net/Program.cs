namespace EraaSoft_Task1.Net
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int smCarpetPrice = 25;
            int lgCarpetPrice = 35;
            double taxRate = 0.06;

            Console.WriteLine("Estimate for carpet cleaning service");

            Console.Write("Number of small carpets: ");
            int smCarpetNum = Convert.ToInt32(Console.ReadLine());

            Console.Write("Number of large carpets: ");
            int lgCarpetNum = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Price per small room: $25");
            Console.WriteLine("Price per large room: $35");

            int totalPrice = (smCarpetPrice * smCarpetNum) + (lgCarpetPrice * lgCarpetNum);
            Console.WriteLine($"Cost :${totalPrice}");

            double totalTax = totalPrice * taxRate;
            Console.WriteLine($"Tax :${totalTax}");

            Console.WriteLine("===============================");

            Console.WriteLine($"Total estimate: ${totalPrice + totalTax}");
            Console.WriteLine("This estimate is valid for 30 days");
        }
    }
}
