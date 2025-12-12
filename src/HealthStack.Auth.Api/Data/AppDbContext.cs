using HealthStack.Auth.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Auth.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        // public DbSet<Address> Addresses => Set<Address>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .OwnsMany(u => u.Addresses, a =>
                {
                    a.WithOwner().HasForeignKey("UserId");
                    a.Property<int>("Id");
                    a.HasKey("Id");
                });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Password = "$2a$11$I/.MeZXNc/fQmKhUk6epu.X9LUaTrANpOYysHu0kpOwcVpc9GFkqG",
                    Role = "User",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    PhoneNumber = "+49 228 123456",
                    CreatedAt = new DateTime(2024, 01, 01)
                }
            );

            modelBuilder.Entity<User>().OwnsMany(u => u.Addresses).HasData(
                new
                {
                    Id = 1,
                    UserId = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                    Street = "Berliner Freiheit 20",
                    City = "Bonn",
                    State = "North Rhine-Westphalia",
                    ZipCode = "53111",
                    Country = "Germany"
                }
            );
        }
    }
}