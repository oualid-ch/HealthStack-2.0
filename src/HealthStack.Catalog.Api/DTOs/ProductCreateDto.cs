namespace HealthStack.Catalog.Api.DTOs
{
    public class ProductCreateDto
    {
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Brand { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
