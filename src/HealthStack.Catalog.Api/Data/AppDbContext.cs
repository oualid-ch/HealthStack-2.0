using HealthStack.Catalog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Catalog.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasPrecision(8, 2);

                // Relationship
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        
            // Seed data
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = Guid.Parse("093bec4a-006f-41b6-a594-e4f236563c3e"),
                    Name = "Supplements",
                    Description = "Health and nutrition supplements"
                },
                new Category
                {
                    Id = Guid.Parse("e978ea0e-68f1-4b62-928f-7034cd56ab7e"),
                    Name = "Hygiene",
                    Description = "Personal hygiene and sanitation products"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = Guid.Parse("78b983cb-d782-42ea-9eec-43b402463a56"),
                    Sku = "HS1001",
                    Name = "Vitamin C 500mg",
                    Brand = "HealthPlus",
                    Description = "Boosts immunity",
                    Price = 12.99m,
                    CategoryId = Guid.Parse("093bec4a-006f-41b6-a594-e4f236563c3e"),
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 12, 01)
                },
                new Product
                {
                    Id = Guid.Parse("813755d7-3f15-47a7-b479-908e281d69c3"),
                    Sku = "HS1002",
                    Name = "Omega-3 Fish Oil",
                    Brand = "NutriLife",
                    Description = "Supports heart and brain health",
                    Price = 19.50m,
                    CategoryId = Guid.Parse("093bec4a-006f-41b6-a594-e4f236563c3e"),
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 12, 01)
                },
                new Product
                {
                    Id = Guid.Parse("d7e749e9-f685-49f9-b855-b0a1684db9db"),
                    Sku = "HS1003",
                    Name = "Hand Sanitizer 250ml",
                    Brand = "SafeHands",
                    Description = "Kills 99.9% germs",
                    Price = 5.75m,
                    CategoryId = Guid.Parse("e978ea0e-68f1-4b62-928f-7034cd56ab7e"),
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 12, 01)
                }
            );
        }
    }
}