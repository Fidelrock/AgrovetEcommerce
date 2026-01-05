using Agrovet.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersCommandController : ControllerBase
{
    private readonly AgrovetDbContext _context;

    public OrdersCommandController(AgrovetDbContext context)
    {
        _context = context;
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        order.Confirm();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        order.Ship();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        order.Deliver();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        order.Cancel();
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
