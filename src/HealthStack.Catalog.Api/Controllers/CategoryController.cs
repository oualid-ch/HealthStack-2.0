using AutoMapper;
using FluentValidation;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;
using HealthStack.Catalog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(
    IValidator<CategoryCreateDto> categoryCreateValidator,
    IValidator<CategoryUpdateDto> categoryUpdateValidator,
    ICategoryService categoryService,
    IMapper mapper
) : ControllerBase
{
    private readonly IValidator<CategoryCreateDto> _categoryCreateValidator = categoryCreateValidator;
    private readonly IValidator<CategoryUpdateDto> _categoryUpdateValidator = categoryUpdateValidator;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IMapper _mapper = mapper;

    // GET ALL CATEGORIES
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetCategories()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(_mapper.Map<IEnumerable<CategoryReadDto>>(categories));
    }

    // POST CATEGORY
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryReadDto>> AddCategory(CategoryCreateDto categoryCreateDto)
    {
        var validation = _categoryCreateValidator.Validate(categoryCreateDto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());
        
        Category category = _mapper.Map<Category>(categoryCreateDto);
        var created =  await _categoryService.AddCategoryAsync(category);

        return CreatedAtAction(nameof(GetCategories), new { id = created.Id }, _mapper.Map<CategoryReadDto>(created));
    }

    // PUT CATEGORY
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryReadDto>> UpdateCategory(Guid id, CategoryUpdateDto categoryUpdateDto)
    {
        var validation = _categoryUpdateValidator.Validate(categoryUpdateDto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());
        
        var created =  await _categoryService.UpdateCategoryAsync(id, categoryUpdateDto);

        return Ok(_mapper.Map<CategoryReadDto>(created));
    }

    // DELETE CATEGORY
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok();
    }

    // RESTORE CATEGORY
    [HttpPut("{id:guid}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RestoreCategory(Guid id)
    {
        await _categoryService.RestoreCategoryAsync(id);
        return Ok();
    }
}