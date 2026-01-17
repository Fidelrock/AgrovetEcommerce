using Agrovet.Application.DTOs.Addresses;
using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressRepository _addressRepository;

    public AddressesController(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    private Guid GetCustomerId()
    {
        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(customerIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses()
    {
        var customerId = GetCustomerId();
        var addresses = await _addressRepository.GetByCustomerIdAsync(customerId);

        var addressDtos = addresses.Select(a => new AddressDto
        {
            Id = a.Id,
            FullName = a.FullName,
            PhoneNumber = a.PhoneNumber,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            State = a.State,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsDefault = a.IsDefault
        }).ToList();

        return Ok(addressDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto request)
    {
        var customerId = GetCustomerId();

        var address = new Address(
            customerId,
            request.FullName,
            request.PhoneNumber,
            request.AddressLine1,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.AddressLine2
        );

        await _addressRepository.AddAsync(address);

        return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, new { message = "Address created successfully." });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] CreateAddressDto request)
    {
        var customerId = GetCustomerId();
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null || address.CustomerId != customerId)
        {
            return NotFound("Address not found.");
        }

        address.Update(
            request.FullName,
            request.PhoneNumber,
            request.AddressLine1,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.AddressLine2
        );

        await _addressRepository.UpdateAsync(address);

        return Ok(new { message = "Address updated successfully." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var customerId = GetCustomerId();
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null || address.CustomerId != customerId)
        {
            return NotFound("Address not found.");
        }

        await _addressRepository.DeleteAsync(address);

        return Ok(new { message = "Address deleted successfully." });
    }

    [HttpPost("{id:guid}/default")]
    public async Task<IActionResult> SetDefaultAddress(Guid id)
    {
        var customerId = GetCustomerId();
        var addresses = await _addressRepository.GetByCustomerIdAsync(customerId);
        var addressToSetDefault = addresses.FirstOrDefault(a => a.Id == id);

        if (addressToSetDefault == null)
        {
            return NotFound("Address not found.");
        }

        foreach (var addr in addresses)
        {
            addr.RemoveDefault();
            await _addressRepository.UpdateAsync(addr);
        }

        addressToSetDefault.SetAsDefault();
        await _addressRepository.UpdateAsync(addressToSetDefault);

        return Ok(new { message = "Default address updated." });
    }
}
