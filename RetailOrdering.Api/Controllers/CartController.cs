using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.Application.DTOs;
using RetailOrdering.Application.Services;
using System.Security.Claims;

namespace RetailOrdering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(CartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid user" });

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart");
            return StatusCode(500, new { message = "An error occurred while fetching cart" });
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid user" });

            var item = await _cartService.AddToCartAsync(userId, request);
            if (item == null)
                return BadRequest(new { message = "Product is unavailable or insufficient stock" });

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to cart");
            return StatusCode(500, new { message = "An error occurred while adding to cart" });
        }
    }

    [HttpPut("items/{id}")]
    public async Task<ActionResult<CartItemDto>> UpdateCartItem(int id, [FromBody] UpdateCartRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid user" });

            var item = await _cartService.UpdateCartItemAsync(userId, id, request);
            if (item == null)
                return NotFound(new { message = "Cart item not found or insufficient stock" });

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item {CartItemId}", id);
            return StatusCode(500, new { message = "An error occurred while updating cart item" });
        }
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid user" });

            var result = await _cartService.RemoveFromCartAsync(userId, id);
            if (!result)
                return NotFound(new { message = "Cart item not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart item {CartItemId}", id);
            return StatusCode(500, new { message = "An error occurred while removing cart item" });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid user" });

            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, new { message = "An error occurred while clearing cart" });
        }
    }
}
