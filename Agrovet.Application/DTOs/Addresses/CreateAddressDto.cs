namespace Agrovet.Application.DTOs.Addresses;

public sealed class CreateAddressDto
{
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}
