namespace Agrovet.Application.DTOs.Cart;

public sealed class CartDto
{
    public Guid Id { get; init; }
    public List<CartItemDto> Items { get; init; } = new();
    public decimal Total { get; init; }
    public int ItemCount { get; init; }
}
