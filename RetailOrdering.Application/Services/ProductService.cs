using RetailOrdering.Application.DTOs;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;

namespace RetailOrdering.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IPackagingRepository _packagingRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IPackagingRepository packagingRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _packagingRepository = packagingRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToProductDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToProductDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByBrandAsync(int brandId)
    {
        var products = await _productRepository.GetByBrandIdAsync(brandId);
        return products.Select(MapToProductDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);
        return products.Select(MapToProductDto);
    }

    public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
    {
        var brands = await _brandRepository.GetAllAsync();
        return brands.Select(MapToBrandDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToCategoryDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesByBrandAsync(int brandId)
    {
        var categories = await _categoryRepository.GetByBrandIdAsync(brandId);
        return categories.Select(MapToCategoryDto);
    }

    public async Task<IEnumerable<PackagingDto>> GetAllPackagingsAsync()
    {
        var packagings = await _packagingRepository.GetAllAsync();
        return packagings.Select(MapToPackagingDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByPackagingAsync(int packagingId)
    {
        var products = await _productRepository.GetAllAsync();
        return products.Where(p => p.PackagingId == packagingId).Select(MapToProductDto);
    }

    private static ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            PackagingId = product.PackagingId,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            Category = product.Category != null ? MapToCategoryDto(product.Category) : null,
            Packaging = product.Packaging != null ? MapToPackagingDto(product.Packaging) : null
        };
    }

    private static CategoryDto MapToCategoryDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            BrandId = category.BrandId,
            Brand = category.Brand != null ? MapToBrandDto(category.Brand) : null
        };
    }

    private static PackagingDto MapToPackagingDto(Packaging packaging)
    {
        return new PackagingDto
        {
            Id = packaging.Id,
            Name = packaging.Name,
            Description = packaging.Description,
            IsActive = packaging.IsActive
        };
    }

    private static BrandDto MapToBrandDto(Brand brand)
    {
        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            ImageUrl = brand.ImageUrl,
            IsActive = brand.IsActive
        };
    }
}
