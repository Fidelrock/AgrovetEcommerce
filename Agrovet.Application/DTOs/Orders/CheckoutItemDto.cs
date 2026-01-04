namespace Agrovet.Application.DTOs.Orders
{
    public sealed class CheckoutItemDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
