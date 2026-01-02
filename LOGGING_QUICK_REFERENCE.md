# Logging Quick Reference

## Inject Logger in Any Class

```csharp
private readonly ILogger<OrderService> _logger;

public OrderService(ILogger<OrderService> logger)
{
    _logger = logger;
}
```

---

## Common Patterns

### Log Business Success
```csharp
_logger.LogInformation("Order {OrderId} shipped successfully", order.Id);
```

### Log Expected Problems (Warning)
```csharp
_logger.LogWarning("Product {ProductId} out of stock", product.Id);
```

### Log Failures (Error)
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process order {OrderId}", order.Id);
    throw;
}
```

### Log for Debugging (Debug)
```csharp
_logger.LogDebug("Validating inventory for {ProductId}", product.Id);
```

### Log Fatal Issues (Fatal)
```csharp
Log.Fatal(ex, "Database connection pool exhausted - application cannot continue");
```

---

## File Output

| File | Who Reads | When |
|------|-----------|------|
| `agrovet-.log` | Ops, PM | Always (production decisions) |
| `agrovet-debug-.log` | Developers | When something is broken |

---

## Red Flags (Code Review)

? If you see this, question it:
- `.LogInformation("Entered method X")`
- `.LogInformation("User input: {@Input}", userInput)` (no business reason)
- `.LogInformation("Setting X = Y")`
- `.LogError(exception)` (no context parameters)

---

## Environment Behavior

### Production
- Logs to `agrovet-.log` (Information+)
- Framework warnings suppressed
- Debug logs discarded

### Development
- Logs to `agrovet-.log` (Information+)
- Logs to `agrovet-debug-.log` (Debug+)
- All levels visible for troubleshooting
