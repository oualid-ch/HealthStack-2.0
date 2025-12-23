using AutoMapper;
using FluentValidation;
using Gridify;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;
using HealthStack.Catalog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(
    IValidator<ProductCreateDto> productCreateValidator, 
    IValidator<ProductUpdateDto> productUpdateValidator,
    IProductService productService,
    IMapper mapper
) : ControllerBase
{
    private readonly IValidator<ProductCreateDto> _productCreateValidator = productCreateValidator;
    private readonly IValidator<ProductUpdateDto> _productUpdateValidator = productUpdateValidator;
    private readonly IProductService _productService = productService;
    private readonly IMapper _mapper = mapper;

    // GET PRODUCT BY ID
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductReadDto>> GetProductById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(_mapper.Map<ProductReadDto>(product));
    }

    // GET PRODUCT BY SKU
    [HttpGet("/sku/{sku}")]
    public async Task<ActionResult<ProductReadDto>> GetProductBySku(string sku)
    {
        var product = await _productService.GetProductBySkuAsync(sku);
        return Ok(_mapper.Map<ProductReadDto>(product));
    }

    // GET ALL PRODUCT
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts([FromQuery] GridifyQuery query)
    {
        var pagedProducts = await _productService.GetProductsAsync(query);
        var result = new {
            // pagedProducts.Count,
            Data = _mapper.Map<IEnumerable<ProductReadDto>>(pagedProducts.Data)
        };
        return Ok(result);
    }
    
    // POST PRODUCT
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductReadDto>> AddProduct(ProductCreateDto productCreateDto)
    {
        var validation = _productCreateValidator.Validate(productCreateDto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());
        
        Product product = _mapper.Map<Product>(productCreateDto);
        var created =  await _productService.AddProductAsync(product);

        return CreatedAtAction(nameof(GetProducts), new { id = created.Id }, _mapper.Map<ProductReadDto>(created));
    }

    // PUT PRODUCT
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductReadDto>> UpdateProduct(Guid id, ProductUpdateDto productUpdateDto)
    {
        var validation = _productUpdateValidator.Validate(productUpdateDto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());
        
        var created =  await _productService.UpdateProductAsync(id, productUpdateDto);

        return Ok(_mapper.Map<ProductReadDto>(created));
    }

    // DELETE PRODUCT
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return Ok();
    }

    // RESTORE PRODUCT
    [HttpPut("{id:guid}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RestoreProduct(Guid id)
    {
        await _productService.RestoreProductAsync(id);
        return Ok();
    }
}