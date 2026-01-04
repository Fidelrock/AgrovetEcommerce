using Agrovet.Api.Contracts.Orders;
using Agrovet.Application.DTOs.Orders;
using Agrovet.Domain.Entities;
using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AgrovetDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    // Add logger for debugging
    public OrdersController(AgrovetDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Select(o => new OrderDto(
                o.Id,
                o.TotalAmount,
                o.Status.ToString(),
                o.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.Product!.Name,
                    i.Quantity,
                    i.UnitPrice,
                    i.GetTotal()
                )).ToList()
            ))
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequestDto request)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            // Validate request
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest(new { error = "Invalid request or empty items" });

            var order = new Order(request.CustomerId);

            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Validate all products exist
            if (products.Count != productIds.Count)
            {
                var foundIds = products.Select(p => p.Id).ToList();
                var missingIds = productIds.Except(foundIds);
                throw new InvalidOperationException($"Products not found: {string.Join(", ", missingIds)}");
            }

            foreach (var item in request.Items)
            {
                var product = products.Single(p => p.Id == item.ProductId);

                _logger.LogInformation("Reducing stock for product {ProductId}: {CurrentStock} - {Quantity}",
                    product.Id, product.StockQuantity, item.Quantity);

                product.ReduceStock(item.Quantity);

                // CRITICAL: Ensure EF tracks this change
                _context.Entry(product).Property(p => p.StockQuantity).IsModified = true;

                order.AddItem(product.Id, product.Price, item.Quantity);
            }

            _context.Orders.Add(order);

            // Save both order and product stock changes
            await _context.SaveChangesAsync();

            order.Confirm();
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            _logger.LogInformation("Checkout successful. Order {OrderId} created with total {Total}",
                order.Id, order.TotalAmount);

            return Ok(new
            {
                orderId = order.Id,
                total = order.TotalAmount,
                message = "Order placed successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            await tx.RollbackAsync();
            _logger.LogWarning(ex, "Business validation failed during checkout");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger.LogError(ex, "Checkout failed unexpectedly");
            return StatusCode(500, new { error = "An unexpected error occurred during checkout" });
        }
    }
}