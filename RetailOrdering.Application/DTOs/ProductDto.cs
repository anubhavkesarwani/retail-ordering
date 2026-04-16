namespace RetailOrdering.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int? PackagingId { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public CategoryDto? Category { get; set; }
    public PackagingDto? Packaging { get; set; }
}
