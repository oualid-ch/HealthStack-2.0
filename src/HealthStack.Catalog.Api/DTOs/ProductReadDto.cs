namespace HealthStack.Catalog.Api.DTOs
{
    public class ProductReadDto
    {
        public Guid Id { get; set; } 
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Brand { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}