using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;

namespace HealthStack.Catalog.Api.Services;

public interface ICategoryService
{
    public Task<IEnumerable<Category>> GetCategoriesAsync();
    public Task<Category> AddCategoryAsync(Category category);
    public Task<Category> UpdateCategoryAsync(Guid id, CategoryUpdateDto category);
    public Task DeleteCategoryAsync(Guid id);
    public Task RestoreCategoryAsync(Guid id);
}