using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class Order : BaseEntity
{
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = "Pending";

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Add(item);
        RecalculateTotal();
        MarkUpdated();
    }

    public void RemoveItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Remove(item);
        RecalculateTotal();
        MarkUpdated();
    }

    public void MarkAsShipped()
    {
        Status = "Shipped";
        MarkUpdated();
    }

    public void MarkAsDelivered()
    {
        Status = "Delivered";
        MarkUpdated();
    }

    public void MarkAsCancelled()
    {
        Status = "Cancelled";
        MarkUpdated();
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(item => item.UnitPrice * item.Quantity);
    }
}
