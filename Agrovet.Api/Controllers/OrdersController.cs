using Agrovet.Api.Contracts.Orders;
using Agrovet.Application.DTOs.Orders;
using OrderItemDtoApi = Agrovet.Api.Contracts.Orders.OrderItemDto;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AgrovetDbContext _context;

    public OrdersController(AgrovetDbContext context)
    {
        _context = context;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
    {
        if (request.Items.Count == 0)
            return BadRequest("Order must contain at least one item.");

        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != productIds.Count)
                return BadRequest("One or more products not found.");

            var order = Order.Create();

            foreach (var item in request.Items)
            {
                var product = products.Single(p => p.Id == item.ProductId);
                product.ReduceStock(item.Quantity);

                // Call AddItem with the 3 required parameters
                order.AddItem(product.Id, item.Quantity, product.Price);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            order.Confirm();
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return Ok(new
            {
                orderId = order.Id,
                total = order.TotalAmount,
                status = order.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /api/orders
    [HttpGet]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        return Ok(orders);
    }

    // GET /api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailsDto>> GetOrderById(Guid id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.Id == id)
            .Select(o => new OrderDetailsDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDtoApi // USE THE ALIAS
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
            return NotFound();

        return Ok(order);
    }
}
