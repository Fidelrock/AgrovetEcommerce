namespace Agrovet.Api.Contracts.Orders;

public record OrderDto(
    Guid Id,
    decimal TotalAmount,
    string Status,
    IReadOnlyCollection<OrderItemDto> Items
);
