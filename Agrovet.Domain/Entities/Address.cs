using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class Address : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string AddressLine1 { get; private set; } = string.Empty;
    public string AddressLine2 { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }

    private Address() { } // EF Core

    public Address(
        Guid customerId,
        string fullName,
        string phoneNumber,
        string addressLine1,
        string city,
        string state,
        string postalCode,
        string country,
        string addressLine2 = "")
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");
        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException("Address line 1 is required.");
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.");

        CustomerId = customerId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        IsDefault = false;
    }

    public void Update(
        string fullName,
        string phoneNumber,
        string addressLine1,
        string city,
        string state,
        string postalCode,
        string country,
        string addressLine2 = "")
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");
        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException("Address line 1 is required.");

        FullName = fullName;
        PhoneNumber = phoneNumber;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        MarkUpdated();
    }

    public void SetAsDefault()
    {
        IsDefault = true;
        MarkUpdated();
    }

    public void RemoveDefault()
    {
        IsDefault = false;
        MarkUpdated();
    }
}
