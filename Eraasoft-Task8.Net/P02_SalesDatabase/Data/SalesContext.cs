using Microsoft.EntityFrameworkCore;
using P02_SalesDatabase.Models;

namespace P02_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext() { }

        public SalesContext(DbContextOptions<SalesContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Task9;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=True;Trust Server Certificate=True;Command Timeout=0");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);
                entity.Property(p => p.Quantity)
                    .HasColumnType("decimal(18,2)");
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");
                entity.Property(p => p.Description)
                    .HasMaxLength(250)
                    .HasDefaultValue("No description");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(true);
                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
                entity.Property(c => c.CreditCardNumber)
                    .IsRequired();
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.StoreId);
                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.SaleId);
                entity.Property(s => s.Date)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(s => s.Product)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Store)
                    .WithMany(st => st.Sales)
                    .HasForeignKey(s => s.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

        public void SeedData()
        {
            if (!Products.Any() && !Customers.Any() && !Stores.Any())
            {
                var random = new Random();

                var products = new[]
                {
                    new Product { Name = "Laptop", Price = 999.99m, Quantity = 50 },
                    new Product { Name = "Mouse", Price = 29.99m, Quantity = 200 },
                    new Product { Name = "Keyboard", Price = 79.99m, Quantity = 150 },
                    new Product { Name = "Monitor", Price = 299.99m, Quantity = 75 },
                    new Product { Name = "Headphones", Price = 149.99m, Quantity = 100 }
                };
                Products.AddRange(products);

                var customers = new[]
                {
                    new Customer { Name = "John Smith", Email = "john.smith@email.com", CreditCardNumber = "1234567890123456" },
                    new Customer { Name = "Jane Doe", Email = "jane.doe@email.com", CreditCardNumber = "2345678901234567" },
                    new Customer { Name = "Bob Johnson", Email = "bob.johnson@email.com", CreditCardNumber = "3456789012345678" },
                    new Customer { Name = "Alice Wilson", Email = "alice.wilson@email.com", CreditCardNumber = "4567890123456789" },
                    new Customer { Name = "Charlie Brown", Email = "charlie.brown@email.com", CreditCardNumber = "5678901234567890" }
                };
                Customers.AddRange(customers);

                var stores = new[]
                {
                    new Store { Name = "Tech Store Downtown" },
                    new Store { Name = "Electronics Mall" },
                    new Store { Name = "Computer World" }
                };
                Stores.AddRange(stores);

                SaveChanges();

                var sales = new List<Sale>();
                for (int i = 0; i < 20; i++)
                {
                    var sale = new Sale
                    {
                        ProductId = products[random.Next(products.Length)].ProductId,
                        CustomerId = customers[random.Next(customers.Length)].CustomerId,
                        StoreId = stores[random.Next(stores.Length)].StoreId
                    };
                    sales.Add(sale);
                }
                Sales.AddRange(sales);
                SaveChanges();

                Console.WriteLine("Sample data has been seeded!");
            }
        }
    }
}
