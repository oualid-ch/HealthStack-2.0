using HealthStack.Order.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Order.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<OrderEntry> Orders => Set<OrderEntry>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderEntry>(entity =>
            {
                entity.Property(o => o.TotalAmount)
                    .HasPrecision(8, 2);

                entity.HasMany(o => o.Items)
                    .WithOne()
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice)
                    .HasPrecision(8, 2);
            });

            // Seed Data
            var orderId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<OrderEntry>().HasData(
                new OrderEntry
                {
                    Id = orderId,
                    UserId = Guid.Parse("58204eb9-fd19-4f73-784f-08de47b1ffe8"),
                    TotalAmount = 49.98m,
                    Status = "Pending",
                    CreatedAt = DateTime.Parse("2024-01-01T10:00:00Z"),
                }
            );

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    OrderId = orderId,
                    ProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    ProductName = "Vitamin C Supplement",
                    UnitPrice = 24.99m,
                    Quantity = 2
                }
            );
        }
    }
}