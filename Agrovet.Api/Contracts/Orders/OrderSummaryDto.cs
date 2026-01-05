namespace Agrovet.Api.Contracts.Orders;

public sealed class OrderSummaryDto
{
    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
