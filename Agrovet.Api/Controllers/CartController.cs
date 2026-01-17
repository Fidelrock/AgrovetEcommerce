using Agrovet.Application.DTOs.Cart;
using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartController(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    private Guid GetCustomerId()
    {
        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(customerIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var customerId = GetCustomerId();
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

        if (cart == null)
        {
            return Ok(new CartDto { Items = new List<CartItemDto>(), Total = 0, ItemCount = 0 });
        }

        var cartDto = new CartDto
        {
            Id = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal
            }).ToList(),
            Total = cart.GetTotal(),
            ItemCount = cart.GetItemCount()
        };

        return Ok(cartDto);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto request)
    {
        var customerId = GetCustomerId();
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            return NotFound("Product not found.");
        }

        if (!product.IsInStock())
        {
            return BadRequest("Product is out of stock.");
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

        if (cart == null)
        {
            cart = new Cart(customerId);
            await _cartRepository.AddAsync(cart);
        }

        cart.AddItem(product.Id, request.Quantity, product.Price);
        await _cartRepository.UpdateAsync(cart);

        return Ok(new { message = "Item added to cart successfully." });
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        var customerId = GetCustomerId();
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

        if (cart == null)
        {
            return NotFound("Cart not found.");
        }

        cart.RemoveItem(productId);
        await _cartRepository.UpdateAsync(cart);

        return Ok(new { message = "Item removed from cart." });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var customerId = GetCustomerId();
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

        if (cart == null)
        {
            return NotFound("Cart not found.");
        }

        cart.Clear();
        await _cartRepository.UpdateAsync(cart);

        return Ok(new { message = "Cart cleared successfully." });
    }
}
