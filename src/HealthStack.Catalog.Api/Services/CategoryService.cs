using HealthStack.Catalog.Api.Auth;
using HealthStack.Catalog.Api.Data;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Exceptions;
using HealthStack.Catalog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Catalog.Api.Services;
public class CategoryService(
    AppDbContext context,
    ILogger<CategoryService> logger,
    ICurrentUser currentUser
) : ICategoryService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<CategoryService> _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
            throw new CategoryNameAlreadyExistsException(category.Name);
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Category created. CategoryId={CategoryId}, AdminId={AdminId}",
            category.Id,
            _currentUser.UserId
        );

        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Guid id, CategoryUpdateDto category)
    {
        var existingCategory = await _context.Categories.FindAsync(id);
        if (existingCategory == null)
        {
            throw new CategoryIdNotFoundException(id);
        }

        _context.Entry(existingCategory).CurrentValues.SetValues(category);
        existingCategory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Category updated. CategoryId={CategoryId}, AdminId={AdminId}",
            id,
            _currentUser.UserId
        );

        return existingCategory;
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var existingCategory = await _context.Categories.FindAsync(id);
        if (existingCategory == null)
        {
            throw new CategoryIdNotFoundException(id);
        }

        existingCategory.IsActive = false;
        existingCategory.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Category deleted. CategoryId={CategoryId}, AdminId={AdminId}",
            id,
            _currentUser.UserId
        );
    }

    public async Task RestoreCategoryAsync(Guid id)
    {
        var existingCategory = await _context.Categories
            // .IgnoreQueryFilters() // important in case of global filters
            .SingleOrDefaultAsync(c => c.Id == id);
        if (existingCategory == null)
            throw new CategoryIdNotFoundException(id);

        if (existingCategory.IsActive)
            return;

        existingCategory.IsActive = true;
        existingCategory.DeletedAt = null;
        existingCategory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Category restored. CategoryId={CategoryId}, AdminId={AdminId}",
            id,
            _currentUser.UserId
        );
    }
}