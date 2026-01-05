using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/products/{productId:guid}/media")]
public class ProductMediaController : ControllerBase
{
    private readonly AgrovetDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductMediaController(AgrovetDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(Guid productId, IFormFile file)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("File is empty.");

        // Save file
        var uploadsFolder = Path.Combine(_env.WebRootPath, "product-media");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        // Create media record
        var media = new ProductMedia(
            productId,
            $"/product-media/{uniqueFileName}",
            "image",      // hardcoded for now, could detect type dynamically
            product.Media.Count + 1 // next display order
        );

        product.AddMedia(media);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { productId }, media);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid productId)
    {
        var media = await _context.Set<ProductMedia>()
            .Where(m => m.ProductId == productId)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        return Ok(media.Select(m => new
        {
            m.Url,
            m.MediaType,
            m.DisplayOrder
        }));
    }
}
