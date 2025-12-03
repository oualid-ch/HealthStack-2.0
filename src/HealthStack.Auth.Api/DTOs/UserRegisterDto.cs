using System.ComponentModel.DataAnnotations;
namespace HealthStack.Auth.Api.DTOs
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }

        public List<AddressCreateDto>? Addresses { get; set; } = [];
    }
}