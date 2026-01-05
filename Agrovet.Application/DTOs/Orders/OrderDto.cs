using Agrovet.Domain.Enums;

namespace Agrovet.Application.DTOs.Orders;

public sealed class OrderDto
{
    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; }

    public List<OrderItemDto> Items { get; init; } = new();
}
