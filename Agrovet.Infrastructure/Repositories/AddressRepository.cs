using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AgrovetDbContext _context;

    public AddressRepository(AgrovetDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        return await _context.Addresses.FindAsync(id);
    }

    public async Task<IEnumerable<Address>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Addresses
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Address address)
    {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}
