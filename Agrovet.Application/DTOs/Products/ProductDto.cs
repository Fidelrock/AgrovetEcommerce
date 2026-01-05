using Agrovet.Application.DTOs.Categories;

namespace Agrovet.Application.DTOs.Products;


public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }
    public bool IsInStock { get; init; }

    public CategoryDto Category { get; init; } = default!;
    public IReadOnlyList<ProductMediaDto> Media { get; init; } = [];
}
