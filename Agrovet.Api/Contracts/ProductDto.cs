using Agrovet.Api.Contracts.Categories;

namespace Agrovet.Api.Contracts;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    CategoryDto Category
);
