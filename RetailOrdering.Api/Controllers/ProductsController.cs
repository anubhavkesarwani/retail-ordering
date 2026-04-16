using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.Application.DTOs;
using RetailOrdering.Application.Services;

namespace RetailOrdering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { message = "An error occurred while fetching products" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the product" });
        }
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
    {
        try
        {
            var brands = await _productService.GetAllBrandsAsync();
            return Ok(brands);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting brands");
            return StatusCode(500, new { message = "An error occurred while fetching brands" });
        }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        try
        {
            var categories = await _productService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, new { message = "An error occurred while fetching categories" });
        }
    }

    [HttpGet("brand/{brandId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByBrand(int brandId)
    {
        try
        {
            var products = await _productService.GetProductsByBrandAsync(brandId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for brand {BrandId}", brandId);
            return StatusCode(500, new { message = "An error occurred while fetching products" });
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId)
    {
        try
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for category {CategoryId}", categoryId);
            return StatusCode(500, new { message = "An error occurred while fetching products" });
        }
    }

    [HttpGet("packagings")]
    public async Task<ActionResult<IEnumerable<PackagingDto>>> GetPackagings()
    {
        try
        {
            var packagings = await _productService.GetAllPackagingsAsync();
            return Ok(packagings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packagings");
            return StatusCode(500, new { message = "An error occurred while fetching packagings" });
        }
    }

    [HttpGet("packaging/{packagingId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByPackaging(int packagingId)
    {
        try
        {
            var products = await _productService.GetProductsByPackagingAsync(packagingId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for packaging {PackagingId}", packagingId);
            return StatusCode(500, new { message = "An error occurred while fetching products" });
        }
    }
}
