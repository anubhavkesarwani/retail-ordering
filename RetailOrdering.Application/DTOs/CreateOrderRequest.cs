namespace RetailOrdering.Application.DTOs;

public class CreateOrderRequest
{
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? DeliveryNotes { get; set; }
}
