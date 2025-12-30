using Gridify;
using HealthStack.Catalog.Api.Auth;
using HealthStack.Catalog.Api.Data;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Exceptions;
using HealthStack.Catalog.Api.Models;
using HealthStack.Catalog.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthStack.Catalog.UnitTests;

public class CategoryServiceUnitTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly CategoryService _sut;

    public CategoryServiceUnitTests()
    {
        // InMemory DB
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Mocks
        _loggerMock = new Mock<ILogger<CategoryService>>();
        _currentUserMock = new Mock<ICurrentUser>();
        // _currentUserMock.Setup(cu => cu.UserId).Returns(Guid.NewGuid());

        // SUT
        _sut = new CategoryService(_context, _loggerMock.Object, _currentUserMock.Object);
    }

    private async Task<Category> SeedActiveCategory()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "A category for testing"
        };
        _context.Categories.Add(category);
        
        await _context.SaveChangesAsync();
        return category;
    }

    private async Task<Category> SeedInactiveCategory()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category 2",
            Description = "A category for testing",
            IsActive = false,
        };
        _context.Categories.Add(category);
        
        await _context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task GetCategoriesAsync_CategoriesExist_ReturnsActiveCategoriesOnly()
    {
        // Arrange
        var activeCategory1 = await SeedActiveCategory();
        var activeCategory2 = await SeedActiveCategory();
        var inactiveCategory = await SeedInactiveCategory();

        // Act
        var result = await _sut.GetCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.All(result, c => Assert.True(c.IsActive));
    }

    [Fact]
    public async Task AddCategoryAsync_NameAvailable_CreatesCategoryAndReturnsIt()
    {
        // Arrange
        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "New Category",
            Description = "A new category",
        };

        // Act
        var result = await _sut.AddCategoryAsync(newCategory);

        // Assert
        Assert.NotNull(result);

        var categoryInDb = await _context.Categories.SingleAsync(c => c.Name == "New Category");
        Assert.Equal(result.Id, categoryInDb.Id);
    }

    [Fact]
    public async Task AddCategoryAsync_NameUnavailable_ThrowsCategoryNameAlreadyExistsException()
    {
        // Arrange
        await SeedActiveCategory();

        var newCategory = new Category
        {
            Name = "Test Category",
            IsActive = true
        };

        // Act
        async Task act() => await _sut.AddCategoryAsync(newCategory);

        // Assert
        await Assert.ThrowsAsync<CategoryNameAlreadyExistsException>(act);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ActiveCategoryExists_UpdatesFields()
    {
        // Arrange
        var category = await SeedActiveCategory();

        var updateDto = new CategoryUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var result = await _sut.UpdateCategoryAsync(category.Id, updateDto);

        // Assert
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateCategoryAsync_InactiveCategoryExists_UpdatesFields()
    {
        // Arrange
        var category = await SeedInactiveCategory();

        var updateDto = new CategoryUpdateDto
        {
            Name = "Updated Inactive Category"
        };

        // Act
        var result = await _sut.UpdateCategoryAsync(category.Id, updateDto);

        // Assert
        Assert.Equal("Updated Inactive Category", result.Name);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateCategoryAsync_CategoryDoesNotExist_ThrowsCategoryIdNotFoundException()
    {
        // Arrange
        var dto = new CategoryUpdateDto { Name = "Does Not Matter" };

        // Act
        async Task act() => await _sut.UpdateCategoryAsync(Guid.NewGuid(), dto);

        // Assert
        await Assert.ThrowsAsync<CategoryIdNotFoundException>(act);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryExists_SoftDeletesCategory()
    {
        // Arrange
        var category = await SeedActiveCategory();

        // Act
        await _sut.DeleteCategoryAsync(category.Id);

        // Assert
        var deletedCategory = await _context.Categories.FindAsync(category.Id);
        Assert.False(deletedCategory!.IsActive);
        Assert.NotNull(deletedCategory.DeletedAt);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryDoesNotExist_ThrowsCategoryIdNotFoundException()
    {
        // Act
        async Task act() => await _sut.DeleteCategoryAsync(Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<CategoryIdNotFoundException>(act);
    }

    [Fact]
    public async Task RestoreCategoryAsync_CategoryExists_ReactivatesCategory()
    {
        // Arrange
        var category = await SeedInactiveCategory();

        // Act
        await _sut.RestoreCategoryAsync(category.Id);

        // Assert
        var restoredCategory = await _context.Categories.FindAsync(category.Id);
        Assert.True(restoredCategory!.IsActive);
        Assert.Null(restoredCategory.DeletedAt);
        Assert.NotNull(restoredCategory.UpdatedAt);
    }

    [Fact]
    public async Task RestoreCategoryAsync_CategoryDoesNotExist_ThrowsCategoryIdNotFoundException()
    {
        // Act
        async Task act() => await _sut.RestoreCategoryAsync(Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<CategoryIdNotFoundException>(act);
    }
}
