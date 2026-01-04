using Agrovet.Domain.Common;
using Agrovet.Domain.Enums;

namespace Agrovet.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    public Order(Guid customerId)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
    }

    public void AddItem(Guid productId, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify order in its current state.");

        var item = new OrderItem(Id,productId, quantity, unitPrice);
        _items.Add(item);

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
        MarkUpdated();
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
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
        TotalAmount = _items.Sum(i => i.UnitPrice * i.Quantity);
    }
}
