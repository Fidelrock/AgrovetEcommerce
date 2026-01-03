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
    public Category Category { get; private set; }
    
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
        Name = name;
        Description = description;
        SKU = sku;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        IsActive = true;
    }

    // ===== Domain behavior =====

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        Price = newPrice;
        MarkUpdated();
    }

    public void AdjustStock(int quantity)
    {
        StockQuantity += quantity;
        MarkUpdated();
    }

    public bool IsInStock() => StockQuantity > 0;

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
