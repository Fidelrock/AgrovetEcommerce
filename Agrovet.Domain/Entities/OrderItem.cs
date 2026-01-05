using Agrovet.Domain.Common;
namespace Agrovet.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }  
    public Order Order { get; private set; } = null!;  

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }  

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem() { } // EF Core

    internal OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        // OrderId will be set by EF Core when added to Order's collection
    }

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Quantity = newQuantity;
        MarkUpdated();
    }

    public decimal GetTotal() => Quantity * UnitPrice;
}