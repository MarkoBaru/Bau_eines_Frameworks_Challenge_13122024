using Microsoft.EntityFrameworkCore;
using BuchShop.Models;


namespace BuchShop.Framework.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguration für die Product-Entität
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductNumber); // Festlegen als Primärschlüssel

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductNumber)
                .ValueGeneratedOnAdd(); // Auto-Increment durch die Datenbank

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Konfiguration für User-CustomerOrder Beziehung
            modelBuilder.Entity<CustomerOrder>()
                .HasOne(co => co.User)
                .WithMany(u => u.CustomerOrders)
                .HasForeignKey(co => co.UserEmail)
                .HasPrincipalKey(u => u.UserEmail);

            modelBuilder.Entity<CustomerOrder>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2); // 18 Stellen insgesamt, davon 2 Nachkommastellen

            // Konfiguration für OrderItem-Product Beziehung
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductNumber)
            .HasPrincipalKey(p => p.ProductNumber);

            // Konfiguration für OrderItem-CustomerOrder Beziehung
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CustomerOrder)
                .WithMany(co => co.OrderItems)
                .HasForeignKey(oi => oi.CustomerOrderId);
            }


    }
}
