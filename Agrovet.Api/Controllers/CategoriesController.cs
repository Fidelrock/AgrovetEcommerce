using Agrovet.Api.Contracts.Categories;
using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AgrovetDbContext _context;

    public CategoriesController(AgrovetDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTree()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.Children)
            .ToListAsync();

        var result = categories.Select(MapCategory).ToList();
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category == null)
            return NotFound();

        return Ok(MapCategory(category));
    }

    private static CategoryDto MapCategory(Domain.Entities.Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Children.Select(MapCategory).ToList()
        );
    }
}
