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
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity > 0,
                p.Media.FirstOrDefault()!.Url ?? string.Empty,
                new CategoryDto(
                    p.Category!.Id,
                    p.Category.Name,
                    p.Category.Slug
                )
            ))
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity > 0,
                p.Media.FirstOrDefault()!.Url ?? string.Empty,
                new CategoryDto(
                    p.Category!.Id,
                    p.Category.Name,
                    p.Category.Slug
                )
            ))
            .FirstOrDefaultAsync();

        if (product is null)
            return NotFound();

        return Ok(product);
    }
}
