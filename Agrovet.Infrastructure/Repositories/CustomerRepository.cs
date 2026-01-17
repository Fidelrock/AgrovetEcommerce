using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AgrovetDbContext _context;

    public CustomerRepository(AgrovetDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Customers.AnyAsync(c => c.Email == email);
    }
}
