using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string SKU { get; private set; } = string.Empty;

    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    // Foreign key
    public Guid CategoryId { get; private set; }

    // Navigation
    public Category? Category { get; private set; }

    private readonly List<ProductMedia> _media = new();
    public IReadOnlyCollection<ProductMedia> Media => _media.AsReadOnly();

    private Product() { } // EF Core

    public Product(
        string name,
        string description,
        string sku,
        decimal price,
        int stockQuantity,
        Guid categoryId)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        if (stockQuantity < 0)
            throw new ArgumentException("Stock cannot be negative.");

        Name = name;
        Description = description;
        SKU = sku;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        IsActive = true;
    }

    // ===== Domain behavior =====

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Invalid quantity.");

        if (StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        StockQuantity -= quantity;
        MarkUpdated();
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Invalid quantity.");

        StockQuantity += quantity;
        MarkUpdated();
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        Price = newPrice;
        MarkUpdated();
    }

    public bool IsInStock() => StockQuantity > 0;

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
