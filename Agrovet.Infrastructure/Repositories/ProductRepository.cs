using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AgrovetDbContext _context;

        public ProductRepository(AgrovetDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
