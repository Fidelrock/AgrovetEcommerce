namespace Agrovet.Application.DTOs.Products;

public sealed class ProductMediaDto
{
    public Guid Id { get; init; }
    public string Url { get; init; } = string.Empty;
    public string MediaType { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}
