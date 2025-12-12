using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Auth.Api.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; } 
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!; // using simple string for simplicity
        public string Role { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Address> Addresses { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}