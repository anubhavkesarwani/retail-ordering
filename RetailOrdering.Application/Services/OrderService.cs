using RetailOrdering.Application.DTOs;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Enums;
using RetailOrdering.Core.Interfaces;

namespace RetailOrdering.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderDto?> CreateOrderAsync(int userId, CreateOrderRequest request)
    {
        // Get user's cart
        var cartItems = (await _cartRepository.GetByUserIdAsync(userId)).ToList();
        if (!cartItems.Any())
            return null;

        // Verify all products are available and have stock
        foreach (var cartItem in cartItems)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null || !product.IsAvailable || product.StockQuantity < cartItem.Quantity)
                return null;
        }

        // Calculate total
        decimal total = 0;
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cartItems)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null) continue;

            total += product.Price * cartItem.Quantity;
            orderItems.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = product.Price,
                Product = product
            });

            // Deduct stock
            product.StockQuantity -= cartItem.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        // Create order
        var order = new Order
        {
            UserId = userId,
            TotalAmount = total,
            DeliveryAddress = request.DeliveryAddress,
            DeliveryNotes = request.DeliveryNotes,
            OrderItems = orderItems
        };

        var created = await _orderRepository.CreateAsync(order);

        // Clear cart
        await _cartRepository.ClearCartAsync(userId);

        return MapToOrderDto(created);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(MapToOrderDto);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToOrderDto);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId)
            return null;

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId)
            return null;

        // Can only cancel Pending or Confirmed orders
        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            return null;

        // Restore stock
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        var updated = await _orderRepository.UpdateAsync(order);

        return updated == null ? null : MapToOrderDto(updated);
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            DeliveryAddress = order.DeliveryAddress,
            DeliveryNotes = order.DeliveryNotes,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}
