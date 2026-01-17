using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { } // EF Core

    public Cart(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required.");

        CustomerId = customerId;
    }

    public void AddItem(Guid productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        var existingItem = _items.SingleOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var cartItem = new CartItem(productId, quantity, price);
            _items.Add(cartItem);
        }

        MarkUpdated();
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var item = _items.SingleOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException("Item not found in cart.");

        item.UpdateQuantity(quantity);
        MarkUpdated();
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.SingleOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            MarkUpdated();
        }
    }

    public void Clear()
    {
        _items.Clear();
        MarkUpdated();
    }

    public decimal GetTotal() => _items.Sum(i => i.Subtotal);

    public int GetItemCount() => _items.Sum(i => i.Quantity);
}
