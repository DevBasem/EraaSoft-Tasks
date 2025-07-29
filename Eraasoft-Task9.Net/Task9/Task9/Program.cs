using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Models;

namespace Task9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new BikeStoresDbContext();
            
            Console.WriteLine("=== LINQ Operations on BikeStore Database ===\n");

            // 1. List all customers' first and last names along with their email addresses
            Console.WriteLine("1. All customers' names and emails:");
            var customers = await context.Customers
                .Select(c => new { c.FirstName, c.LastName, c.Email })
                .ToListAsync();
            
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.FirstName} {customer.LastName} - {customer.Email}");
            }
            Console.WriteLine();

            // 2. Retrieve all orders processed by a specific staff member (e.g., staff_id = 3)
            Console.WriteLine("2. Orders processed by staff member with ID 3:");
            var staffOrders = await context.Orders
                .Where(o => o.StaffId == 3)
                .Include(o => o.Customer)
                .ToListAsync();
            
            foreach (var order in staffOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.Customer?.FirstName} {order.Customer?.LastName}, Date: {order.OrderDate:yyyy-MM-dd}");
            }
            Console.WriteLine();

            // 3. Get all products that belong to a category named "Mountain Bikes"
            Console.WriteLine("3. Products in 'Mountain Bikes' category:");
            var mountainBikes = await context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.CategoryName == "Mountain Bikes")
                .ToListAsync();
            
            foreach (var product in mountainBikes)
            {
                Console.WriteLine($"{product.ProductName} - ${product.ListPrice}");
            }
            Console.WriteLine();

            // 4. Count the total number of orders per store
            Console.WriteLine("4. Total number of orders per store:");
            var ordersPerStore = await context.Orders
                .Include(o => o.Store)
                .GroupBy(o => o.Store.StoreName)
                .Select(g => new { StoreName = g.Key, OrderCount = g.Count() })
                .ToListAsync();
            
            foreach (var store in ordersPerStore)
            {
                Console.WriteLine($"{store.StoreName}: {store.OrderCount} orders");
            }
            Console.WriteLine();

            // 5. List all orders that have not been shipped yet (shipped_date is null)
            Console.WriteLine("5. Orders not yet shipped:");
            var unshippedOrders = await context.Orders
                .Where(o => o.ShippedDate == null)
                .Include(o => o.Customer)
                .ToListAsync();
            
            foreach (var order in unshippedOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.Customer?.FirstName} {order.Customer?.LastName}, Order Date: {order.OrderDate:yyyy-MM-dd}");
            }
            Console.WriteLine();

            // 6. Display each customer's full name and the number of orders they have placed
            Console.WriteLine("6. Customers and their order counts:");
            var customerOrderCounts = await context.Customers
                .Select(c => new 
                { 
                    FullName = c.FirstName + " " + c.LastName,
                    OrderCount = c.Orders.Count()
                })
                .ToListAsync();
            
            foreach (var customer in customerOrderCounts)
            {
                Console.WriteLine($"{customer.FullName}: {customer.OrderCount} orders");
            }
            Console.WriteLine();

            // 7. List all products that have never been ordered (not found in order_items)
            Console.WriteLine("7. Products never ordered:");
            var neverOrderedProducts = await context.Products
                .Where(p => !p.OrderItems.Any())
                .ToListAsync();
            
            foreach (var product in neverOrderedProducts)
            {
                Console.WriteLine($"{product.ProductName} - ${product.ListPrice}");
            }
            Console.WriteLine();

            // 8. Display products that have a quantity of less than 5 in any store stock
            Console.WriteLine("8. Products with quantity less than 5 in any store:");
            var lowStockProducts = await context.Stocks
                .Where(s => s.Quantity < 5)
                .Include(s => s.Product)
                .Include(s => s.Store)
                .Select(s => new { s.Product.ProductName, s.Store.StoreName, s.Quantity })
                .ToListAsync();
            
            foreach (var stock in lowStockProducts)
            {
                Console.WriteLine($"{stock.ProductName} at {stock.StoreName}: {stock.Quantity} units");
            }
            Console.WriteLine();

            // 9. Retrieve the first product from the products table
            Console.WriteLine("9. First product:");
            var firstProduct = await context.Products.FirstOrDefaultAsync();
            if (firstProduct != null)
            {
                Console.WriteLine($"{firstProduct.ProductName} - ${firstProduct.ListPrice}");
            }
            Console.WriteLine();

            // 10. Retrieve all products from the products table with a certain model year
            Console.WriteLine("10. Products from model year 2018:");
            var products2018 = await context.Products
                .Where(p => p.ModelYear == 2018)
                .ToListAsync();
            
            foreach (var product in products2018)
            {
                Console.WriteLine($"{product.ProductName} - Model Year: {product.ModelYear}");
            }
            Console.WriteLine();

            // 11. Display each product with the number of times it was ordered
            Console.WriteLine("11. Products and their order counts:");
            var productOrderCounts = await context.Products
                .Select(p => new 
                { 
                    p.ProductName,
                    OrderCount = p.OrderItems.Count()
                })
                .ToListAsync();
            
            foreach (var product in productOrderCounts)
            {
                Console.WriteLine($"{product.ProductName}: ordered {product.OrderCount} times");
            }
            Console.WriteLine();

            // 12. Count the number of products in a specific category
            Console.WriteLine("12. Number of products in 'Road Bikes' category:");
            var roadBikeCount = await context.Products
                .Include(p => p.Category)
                .CountAsync(p => p.Category.CategoryName == "Road Bikes");
            
            Console.WriteLine($"Road Bikes category has {roadBikeCount} products");
            Console.WriteLine();

            // 13. Calculate the average list price of products
            Console.WriteLine("13. Average list price of products:");
            var averagePrice = await context.Products.AverageAsync(p => p.ListPrice);
            Console.WriteLine($"Average price: ${averagePrice:F2}");
            Console.WriteLine();

            // 14. Retrieve a specific product from the products table by ID
            Console.WriteLine("14. Product with ID 1:");
            var specificProduct = await context.Products
                .Where(p => p.ProductId == 1)
                .FirstOrDefaultAsync();
            
            if (specificProduct != null)
            {
                Console.WriteLine($"{specificProduct.ProductName} - ${specificProduct.ListPrice}");
            }
            Console.WriteLine();

            // 15. List all products that were ordered with a quantity greater than 3 in any order
            Console.WriteLine("15. Products ordered with quantity > 3:");
            var highQuantityProducts = await context.OrderItems
                .Where(oi => oi.Quantity > 3)
                .Include(oi => oi.Product)
                .Select(oi => oi.Product)
                .Distinct()
                .ToListAsync();
            
            foreach (var product in highQuantityProducts)
            {
                Console.WriteLine($"{product.ProductName} - ${product.ListPrice}");
            }
            Console.WriteLine();

            // 16. Display each staff member's name and how many orders they processed
            Console.WriteLine("16. Staff members and their processed order counts:");
            var staffOrderCounts = await context.Staffs
                .Select(s => new 
                { 
                    FullName = s.FirstName + " " + s.LastName,
                    OrderCount = s.Orders.Count()
                })
                .ToListAsync();
            
            foreach (var staff in staffOrderCounts)
            {
                Console.WriteLine($"{staff.FullName}: processed {staff.OrderCount} orders");
            }
            Console.WriteLine();

            // 17. List active staff members only (active = true) along with their phone numbers
            Console.WriteLine("17. Active staff members with phone numbers:");
            var activeStaff = await context.Staffs
                .Where(s => s.Active == 1)
                .Select(s => new { FullName = s.FirstName + " " + s.LastName, s.Phone })
                .ToListAsync();
            
            foreach (var staff in activeStaff)
            {
                Console.WriteLine($"{staff.FullName} - Phone: {staff.Phone ?? "N/A"}");
            }
            Console.WriteLine();

            // 18. List all products with their brand name and category name
            Console.WriteLine("18. Products with brand and category names:");
            var productsWithDetails = await context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Select(p => new 
                { 
                    p.ProductName, 
                    BrandName = p.Brand.BrandName, 
                    CategoryName = p.Category.CategoryName 
                })
                .ToListAsync();
            
            foreach (var product in productsWithDetails)
            {
                Console.WriteLine($"{product.ProductName} - Brand: {product.BrandName}, Category: {product.CategoryName}");
            }
            Console.WriteLine();

            // 19. Retrieve orders that are completed
            Console.WriteLine("19. Completed orders (assuming order_status = 4 means completed):");
            var completedOrders = await context.Orders
                .Where(o => o.OrderStatus == 4)
                .Include(o => o.Customer)
                .ToListAsync();
            
            foreach (var order in completedOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.Customer?.FirstName} {order.Customer?.LastName}, Date: {order.OrderDate:yyyy-MM-dd}");
            }
            Console.WriteLine();

            // 20. List each product with the total quantity sold (sum of quantity from order_items)
            Console.WriteLine("20. Products with total quantity sold:");
            var productTotalSold = await context.Products
                .Select(p => new 
                { 
                    p.ProductName,
                    TotalSold = p.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();
            
            foreach (var product in productTotalSold)
            {
                Console.WriteLine($"{product.ProductName}: {product.TotalSold} units sold");
            }
            Console.WriteLine();

            Console.WriteLine("=== All LINQ operations completed! ===");
        }
    }
}
