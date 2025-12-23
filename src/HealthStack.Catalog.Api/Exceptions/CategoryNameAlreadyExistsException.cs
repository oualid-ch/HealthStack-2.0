namespace HealthStack.Catalog.Api.Exceptions;
public class CategoryNameAlreadyExistsException(string name) : Exception($"Category name '{name}' already exists")
{
    public string Name = name;
}
