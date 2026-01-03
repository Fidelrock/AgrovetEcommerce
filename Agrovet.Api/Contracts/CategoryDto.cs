namespace Agrovet.Api.Contracts;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug
);
