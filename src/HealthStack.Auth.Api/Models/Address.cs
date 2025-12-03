using System.ComponentModel.DataAnnotations;

namespace HealthStack.Auth.Api.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Street { get; set; } = default!;

        [Required, MaxLength(50)]
        public string City { get; set; } = default!;

        [Required, MaxLength(50)]
        public string State { get; set; } = default!;

        [Required, MaxLength(20)]
        public string ZipCode { get; set; } = default!;

        [Required, MaxLength(50)]
        public string Country { get; set; } = default!;
    }
}