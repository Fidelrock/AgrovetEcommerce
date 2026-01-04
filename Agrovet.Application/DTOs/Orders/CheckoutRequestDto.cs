namespace Agrovet.Application.DTOs.Orders
{
    public sealed class CheckoutRequestDto
    {
        public Guid CustomerId { get; init; }
        public List<CheckoutItemDto> Items { get; init; } = new();
    }
}
