namespace HealthStack.Catalog.Api.Exceptions;
public class CategoryIdNotFoundException(Guid categoryId) : Exception($"Category not found for Id {categoryId}")
{
    public Guid CategoryId { get; } = categoryId;
}