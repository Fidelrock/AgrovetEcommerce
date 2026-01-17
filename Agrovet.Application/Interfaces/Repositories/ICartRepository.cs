using Agrovet.Domain.Entities;

namespace Agrovet.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByCustomerIdAsync(Guid customerId);
    Task<Cart?> GetByIdAsync(Guid id);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(Cart cart);
}
