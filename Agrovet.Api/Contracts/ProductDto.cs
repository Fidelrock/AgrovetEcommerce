namespace Agrovet.Api.Contracts;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    bool InStock,
    string ImageUrl,
    CategoryDto Category
);
