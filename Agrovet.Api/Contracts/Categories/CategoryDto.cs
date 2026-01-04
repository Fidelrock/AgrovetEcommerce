namespace Agrovet.Api.Contracts.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    IReadOnlyCollection<CategoryDto> Children
);
