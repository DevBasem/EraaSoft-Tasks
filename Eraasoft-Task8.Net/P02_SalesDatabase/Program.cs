using P02_SalesDatabase.Data;

namespace P02_SalesDatabase
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("P02_SalesDatabase Application");

            using var context = new SalesContext();

            context.Database.EnsureCreated();

            context.SeedData();

            Console.WriteLine("Database created and seeded successfully!");
            Console.WriteLine($"Products: {context.Products.Count()}");
            Console.WriteLine($"Customers: {context.Customers.Count()}");
            Console.WriteLine($"Stores: {context.Stores.Count()}");
            Console.WriteLine($"Sales: {context.Sales.Count()}");
        }
    }
}
