using Agrovet.Api.Contracts;
using Agrovet.Application.DTOs.Products;
using Agrovet.Application.DTOs.Categories;
using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductDtoApp = Agrovet.Application.DTOs.Products.ProductDto;

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
            .Include(p => p.Media)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            query = query.Where(p => p.Category!.Slug == categorySlug);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDtoApp
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsInStock = p.StockQuantity > 0,
                Category = new CategoryDto
                {
                    Id = p.Category!.Id,
                    Name = p.Category.Name,
                    Slug = p.Category.Slug
                },
                Media = p.Media
                    .OrderBy(m => m.DisplayOrder)
                    .Select(m => new ProductMediaDto
                    {
                        Id = m.Id,
                        Url = m.Url,
                        MediaType = m.MediaType,
                        DisplayOrder = m.DisplayOrder
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(new PagedResult<ProductDtoApp>(
            products,
            page,
            pageSize,
            totalCount
        ));
    }

}
