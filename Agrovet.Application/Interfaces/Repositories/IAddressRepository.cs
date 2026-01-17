using Agrovet.Domain.Entities;

namespace Agrovet.Application.Interfaces.Repositories;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id);
    Task<IEnumerable<Address>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(Address address);
    Task UpdateAsync(Address address);
    Task DeleteAsync(Address address);
}
