using System.ComponentModel.DataAnnotations;
namespace HealthStack.Auth.Api.DTOs
{   
    public class UserLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(6)]
        public string Password { get; set; } = default!;
    }
}