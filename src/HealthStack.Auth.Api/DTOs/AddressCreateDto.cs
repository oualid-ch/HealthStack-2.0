namespace HealthStack.Auth.Api.DTOs
{
    public class AddressCreateDto
    {
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string Country { get; set; } = default!;
    }
}
