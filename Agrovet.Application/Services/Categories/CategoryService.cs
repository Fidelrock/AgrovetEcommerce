using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Agrovet.Application.Services.Categories;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Category> CreateAsync(
        string name,
        string slug,
        Guid? parentCategoryId = null)
    {
        _logger.LogInformation("Creating category {CategoryName}", name);

        var category = new Category(name, slug, parentCategoryId);

        await _categoryRepository.AddAsync(category);

        _logger.LogInformation(
            "Category {CategoryId} created successfully",
            category.Id);

        return category;
    }
}
