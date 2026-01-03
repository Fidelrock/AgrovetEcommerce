public sealed class CategoryTreeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public List<CategoryTreeDto> Children { get; init; } = new();
}
