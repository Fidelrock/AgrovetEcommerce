using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly AgrovetDbContext _context;

        public CategoriesController(AgrovetDbContext context)
        {
            _context = context;
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            var lookup = categories.ToLookup(c => c.ParentCategoryId);

            List<CategoryTreeDto> BuildTree(Guid? parentId)
            {
                return lookup[parentId]
                    .Select(c => new CategoryTreeDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Slug = c.Slug,
                        Children = BuildTree(c.Id)
                    })
                    .ToList();
            }

            return Ok(BuildTree(null));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category is null)
                return NotFound();

            return Ok(new CategoryDetailsDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug
            });
        }
    }
}
