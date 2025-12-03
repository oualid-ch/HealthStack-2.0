using HealthStack.Auth.Api.Models;

namespace HealthStack.Auth.Api.DTOs
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Address> Addresses { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}