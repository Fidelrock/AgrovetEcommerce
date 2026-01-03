using Agrovet.Api.Contracts;
using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AgrovetDbContext _context;

    public ProductsController(AgrovetDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? categorySlug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 50) pageSize = 12;

        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            query = query.Where(p =>
                p.Category!.Slug == categorySlug);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(
    p.Id,
    p.Name,
    p.Description,
    p.Price,
    new CategoryDto(
        p.Category!.Id,
        p.Category.Name,
        p.Category.Slug
    )
))

            .ToListAsync();

        return Ok(new PagedResult<ProductDto>(
            products,
            page,
            pageSize,
            totalCount
        ));
    }
}
