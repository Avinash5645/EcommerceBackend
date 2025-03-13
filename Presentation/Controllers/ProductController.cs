using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize] // Requires authentication for all endpoints
[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<Category> _categoryRepository;

    public ProductController(IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    [AllowAnonymous] // Open to public
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        }).ToList();

        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // Open to public
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };

        return Ok(productDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Restricted to Admin only
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] AddProductDto addProductDto)
    {
        // Check if the provided CategoryId exists
        var categoryExists = await _categoryRepository.GetByIdAsync(addProductDto.CategoryId);
        if (categoryExists == null)
        {
            return BadRequest($"Category with Id {addProductDto.CategoryId} does not exist.");
        }

        var product = new Product
        {
            Name = addProductDto.Name,
            Description = addProductDto.Description,
            Price = addProductDto.Price,
            ImageUrl = addProductDto.ImageUrl,
            CategoryId = addProductDto.CategoryId
        };

        await _productRepository.AddAsync(product);

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null) return NotFound();

        // Check if the provided CategoryId exists
        var categoryExists = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId);
        if (categoryExists == null)
        {
            return BadRequest($"Category with Id {updateProductDto.CategoryId} does not exist.");
        }

        existingProduct.Name = updateProductDto.Name;
        existingProduct.Description = updateProductDto.Description;
        existingProduct.Price = updateProductDto.Price;
        existingProduct.ImageUrl = updateProductDto.ImageUrl;
        existingProduct.CategoryId = updateProductDto.CategoryId;

        _productRepository.Update(existingProduct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();

        _productRepository.Delete(product);
        return NoContent();
    }
}
