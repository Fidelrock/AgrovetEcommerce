using Agrovet.Domain.Entities;

namespace Agrovet.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task<bool> EmailExistsAsync(string email);
}
