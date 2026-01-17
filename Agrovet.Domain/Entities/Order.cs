using Agrovet.Domain.Common;
using Agrovet.Domain.Enums;

namespace Agrovet.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public Guid? ShippingAddressId { get; private set; }
    public Address? ShippingAddress { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public string? TrackingNumber { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create()
    {
        return new Order();
    }

    public Order(Guid customerId)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        ShippingCost = 0;
    }

    public void SetShippingDetails(Guid addressId, decimal shippingCost)
    {
        if (shippingCost < 0)
            throw new ArgumentException("Shipping cost cannot be negative.");

        ShippingAddressId = addressId;
        ShippingCost = shippingCost;
        RecalculateTotal();
        MarkUpdated();
    }

    public void UpdateTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number is required.");

        TrackingNumber = trackingNumber;
        MarkUpdated();
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify order in its current state.");

        var existingItem = _items.SingleOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = new OrderItem(productId, quantity, unitPrice);
            _items.Add(item);
        }

        RecalculateTotal();
        MarkUpdated();
    }


    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");

        if (!_items.Any())
            throw new InvalidOperationException("Order cannot be empty.");

        Status = OrderStatus.Confirmed;
        MarkUpdated();
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped.");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        MarkUpdated();
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        MarkUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
        MarkUpdated();
    }

    private void RecalculateTotal()
    {
        Subtotal = _items.Sum(i => i.UnitPrice * i.Quantity);
        TotalAmount = Subtotal + ShippingCost;
    }
}
