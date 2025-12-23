using Microsoft.EntityFrameworkCore;

namespace HealthStack.Catalog.Api.Models;

[Index(nameof(Sku), IsUnique = true)]
public class Product
{
    public Guid Id { get; set; } 
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Brand { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}