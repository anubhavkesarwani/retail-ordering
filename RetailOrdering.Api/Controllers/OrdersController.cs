using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.Application.DTOs;
using RetailOrdering.Application.Services;
using RetailOrdering.Core.Enums;
using System.Security.Claims;

namespace RetailOrdering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private bool IsAdmin()
    {
        return User.IsInRole(UserRole.Admin.ToString());
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var order = await _orderService.CreateOrderAsync(userId, request);
            if (order == null)
            {
                return BadRequest(new { message = "Unable to create order. Cart may be empty or products unavailable." });
            }

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { message = "An error occurred while creating order" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        try
        {
            if (IsAdmin())
            {
                var allOrders = await _orderService.GetAllOrdersAsync();
                return Ok(allOrders);
            }

            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return StatusCode(500, new { message = "An error occurred while fetching orders" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        try
        {
            if (IsAdmin())
            {
                var order = await _orderService.GetAllOrdersAsync();
                var found = order.FirstOrDefault(o => o.Id == id);
                if (found == null)
                {
                    return NotFound(new { message = "Order not found" });
                }
                return Ok(found);
            }

            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var userOrder = await _orderService.GetOrderByIdAsync(id, userId);
            if (userOrder == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(userOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching order" });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<OrderDto>> CancelOrder(int id)
    {
        try
        {
            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var order = await _orderService.CancelOrderAsync(id, userId);
            if (order == null)
            {
                return BadRequest(new { message = "Unable to cancel order. Order may not exist or cannot be cancelled." });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(500, new { message = "An error occurred while cancelling order" });
        }
    }
}
