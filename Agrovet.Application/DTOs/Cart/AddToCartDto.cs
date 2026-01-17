namespace Agrovet.Application.DTOs.Cart;

public sealed class AddToCartDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
