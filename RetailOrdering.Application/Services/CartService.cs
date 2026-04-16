using RetailOrdering.Application.DTOs;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;

namespace RetailOrdering.Application.Services;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
    {
        var cartItems = await _cartRepository.GetByUserIdAsync(userId);
        return cartItems.Select(MapToCartItemDto);
    }

    public async Task<CartItemDto?> AddToCartAsync(int userId, AddToCartRequest request)
    {
        // Check if product exists and is available
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null || !product.IsAvailable || product.StockQuantity < request.Quantity)
            return null;

        // Check if item already exists in cart
        var existingItem = await _cartRepository.GetByUserAndProductAsync(userId, request.ProductId);

        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
            var updated = await _cartRepository.UpdateAsync(existingItem);
            return updated == null ? null : MapToCartItemDto(updated);
        }

        // Create new cart item
        var cartItem = new CartItem
        {
            UserId = userId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var created = await _cartRepository.CreateAsync(cartItem);
        return MapToCartItemDto(created);
    }

    public async Task<CartItemDto?> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartRequest request)
    {
        var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
        if (cartItem == null || cartItem.UserId != userId)
            return null;

        // Check if product has enough stock
        var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
        if (product == null || product.StockQuantity < request.Quantity)
            return null;

        cartItem.Quantity = request.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cartItem);
        return updated == null ? null : MapToCartItemDto(updated);
    }

    public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
    {
        var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
        if (cartItem == null || cartItem.UserId != userId)
            return false;

        return await _cartRepository.DeleteAsync(cartItemId);
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        return await _cartRepository.ClearCartAsync(userId);
    }

    private static CartItemDto MapToCartItemDto(CartItem cartItem)
    {
        return new CartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            Product = cartItem.Product != null ? MapToProductDto(cartItem.Product) : null
        };
    }

    private static ProductDto MapToProductDto(Core.Entities.Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable
        };
    }
}
