using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;

    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }

    // Navigation
    private readonly List<Category> _children = new();
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { } // EF Core

    public Category(string name, string slug, Guid? parentCategoryId = null)
    {
        Name = name;
        Slug = slug;
        ParentCategoryId = parentCategoryId;
    }

    public void Rename(string name, string slug)
    {
        Name = name;
        Slug = slug;
        MarkUpdated();
    }
}
