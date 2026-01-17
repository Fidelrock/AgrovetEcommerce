using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AgrovetDbContext _context;

    public CartRepository(AgrovetDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Cart?> GetByIdAsync(Guid id)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Cart cart)
    {
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();
    }
}
