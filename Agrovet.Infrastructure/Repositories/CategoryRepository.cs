using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AgrovetDbContext _context;

        public CategoryRepository(AgrovetDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Children)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
