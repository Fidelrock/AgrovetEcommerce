using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class Customer : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private Customer() { } // EF Core

    public Customer(string email, string fullName, string phoneNumber, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.");

        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
    }

    public void UpdateProfile(string fullName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");

        FullName = fullName;
        PhoneNumber = phoneNumber;
        MarkUpdated();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required.");

        PasswordHash = newPasswordHash;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }

    public void AddAddress(Address address)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        _addresses.Add(address);
        MarkUpdated();
    }
}
