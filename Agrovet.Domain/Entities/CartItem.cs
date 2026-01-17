using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Cart Cart { get; private set; } = null!;

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Subtotal => Quantity * UnitPrice;

    private CartItem() { } // EF Core

    internal CartItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Quantity = newQuantity;
        MarkUpdated();
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative.");

        UnitPrice = newPrice;
        MarkUpdated();
    }
}
