using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthStack.Catalog.Api.Controllers;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;
using HealthStack.Catalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthStack.Catalog.UnitTests;

public class CategoryControllerUnitTests
{
    private readonly Mock<IValidator<CategoryCreateDto>> _categoryCreateValidator = new();
    private readonly Mock<IValidator<CategoryUpdateDto>> _categoryUpdateValidator = new();
    private readonly Mock<ICategoryService> _categoryServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();


    private CategoryController CreateController()
        => new(
            _categoryCreateValidator.Object,
            _categoryUpdateValidator.Object,
            _categoryServiceMock.Object,
            _mapperMock.Object
        );

    [Fact]
    public async Task GetCategories_ReturnsOkWithMappedProducts()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Category1" },
            new() { Id = Guid.NewGuid(), Name = "Category2" }
        };

        var categoryDtos = new List<CategoryReadDto>
        {
            new() { Id = categories[0].Id, Name = "Category1" },
            new() { Id = categories[1].Id, Name = "Category2" }
        };

        _categoryServiceMock
            .Setup(s => s.GetCategoriesAsync())
            .ReturnsAsync(categories);
        
        _mapperMock
            .Setup(m => m.Map<IEnumerable<CategoryReadDto>>(categories))
            .Returns(categoryDtos);
        
        var controller = CreateController();
        
        // Act
        var result = await controller.GetCategories();
        
        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        ok.Value.Should().BeEquivalentTo(categoryDtos);
    }


    [Fact]
    public async Task AddProduct_ValidModel_ReturnsCreated()
    {
        var dto = new CategoryCreateDto { Name = "Test" };
        var category = new Category { Id = Guid.NewGuid() };
        var readDto = new CategoryReadDto { Id = category.Id };

        _categoryCreateValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());

        _mapperMock
            .Setup(m => m.Map<Category>(dto))
            .Returns(category);

        _categoryServiceMock
            .Setup(s => s.AddCategoryAsync(category))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(m => m.Map<CategoryReadDto>(category))
            .Returns(readDto);

        var controller = CreateController();

        var result = await controller.AddCategory(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        created.Value.Should().BeEquivalentTo(readDto);
    }

    [Fact]
    public async Task AddProduct_InvalidModel_ReturnsBadRequest()
    {
        var dto = new CategoryCreateDto();

        _categoryCreateValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult(
                [ new ValidationFailure("Name", "Required") ]
            ));

        var controller = CreateController();

        var result = await controller.AddCategory(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_ValidModel_ReturnsOk()
    {
        var dto = new CategoryUpdateDto { Name = "Updated" };
        var category = new Category { Id = Guid.NewGuid(), Name = "Updated" };
        var readDto = new CategoryReadDto { Id = category.Id };

        _categoryUpdateValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());

        _categoryServiceMock
            .Setup(s => s.UpdateCategoryAsync(category.Id, dto))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(m => m.Map<CategoryReadDto>(category))
            .Returns(readDto);

        var controller = CreateController();

        var result = await controller.UpdateCategory(category.Id, dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        ok.Value.Should().BeEquivalentTo(readDto);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var controller = CreateController();

        var result = await controller.DeleteCategory(id);

        Assert.IsType<OkResult>(result);
        _categoryServiceMock.Verify(s => s.DeleteCategoryAsync(id), Times.Once);
    }

    [Fact]
    public async Task RestoreProduct_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var controller = CreateController();

        var result = await controller.RestoreCategory(id);

        Assert.IsType<OkResult>(result);
        _categoryServiceMock.Verify(s => s.RestoreCategoryAsync(id), Times.Once);
    }
}
