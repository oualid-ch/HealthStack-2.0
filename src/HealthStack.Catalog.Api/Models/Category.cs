using Microsoft.EntityFrameworkCore;

namespace HealthStack.Catalog.Api.Models;
[Index(nameof(Name), IsUnique = true)]
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public ICollection<Product> Products { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}