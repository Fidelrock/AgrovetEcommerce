namespace Agrovet.Application.DTOs.Orders;

public sealed class CheckoutRequestDto
{
    public List<CheckoutItemDto> Items { get; init; } = new();
}

