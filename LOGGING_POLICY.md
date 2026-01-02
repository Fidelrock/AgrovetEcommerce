# Agrovet Logging Policy

## Overview
Logging must be **intentional, leveled, and actionable**. We log by business impact, not verbosity.

---

## Log Levels

| Level | Purpose | Example |
|-------|---------|---------|
| **Information** | Business events & lifecycle milestones | App startup, order created, payment processed |
| **Debug** | Step-by-step execution (dev diagnostics) | Entering service, validating input, payload size |
| **Warning** | Unexpected but recoverable situations | Invalid input, retry attempts, deprecated API usage |
| **Error** | Failures that need attention | Database failures, payment errors, validation errors |
| **Fatal** | Application cannot continue | Startup crash, unrecoverable infrastructure failure |

---

## What We DO Log

### Controllers
```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
{
    _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);
    
    try
    {
        var order = await _service.CreateOrderAsync(request);
        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning(ex, "Invalid order creation request for customer {CustomerId}", request.CustomerId);
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create order for customer {CustomerId}", request.CustomerId);
        return StatusCode(500);
    }
}
```

### Services / Business Logic
```csharp
public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
{
    _logger.LogInformation(
        "Processing order with {ItemCount} items for customer {CustomerId}",
        request.Items.Count,
        request.CustomerId
    );

    try
    {
        var order = new Order { CustomerId = request.CustomerId };
        await _repository.AddAsync(order);
        
        _logger.LogInformation(
            "Order {OrderId} persisted successfully",
            order.Id
        );
        
        return order;
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Failed to process order for customer {CustomerId}",
            request.CustomerId
        );
        throw;
    }
}
```

### Infrastructure (Repositories)
```csharp
public async Task<Product> GetByIdAsync(int id)
{
    try
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Database error retrieving product {ProductId}", id);
        throw;
    }
}
```

### Debug Logs (When Tracing)
```csharp
_logger.LogDebug(
    "Validating product SKU {Sku} against inventory",
    product.Sku
);
```

---

## What We DO NOT Log

? **Entry/Exit logs**
```csharp
// WRONG
_logger.LogInformation("Entered CreateOrder method");
_logger.LogInformation("Exiting CreateOrder method");
```

? **Every variable assignment**
```csharp
// WRONG
_logger.LogInformation("Created customer object");
_logger.LogInformation("Set order status to pending");
```

? **Sensitive data**
```csharp
// WRONG
_logger.LogInformation("User {Email} logged in with password {Password}", email, password);

// RIGHT
_logger.LogInformation("User {Email} logged in successfully", email);
```

? **Request/Response bodies at Info level**
```csharp
// WRONG
_logger.LogInformation("Request body: {@Request}", request);

// RIGHT (Debug only)
_logger.LogDebug("Processing request: {@Request}", request);
```

---

## Log Output Strategy

### `agrovet-.log` (Production Review)
- **Level:** Information and above
- **Contents:** Business events only
- **Retention:** 7 days
- **Use:** Prod monitoring, audit trails, business intelligence

**Example Output:**
```
2025-01-24 14:32:45.123 +01:00 [INF] Starting web application
2025-01-24 14:32:47.456 +01:00 [INF] Database initialization completed
2025-01-24 14:33:02.789 +01:00 [INF] Creating order for customer 42
2025-01-24 14:33:03.012 +01:00 [INF] Order 108 created successfully
2025-01-24 14:33:15.345 +01:00 [WRN] Invalid order creation request for customer 42
2025-01-24 14:33:22.678 +01:00 [ERR] Failed to create order for customer 42
```

### `agrovet-debug-.log` (Development)
- **Level:** Debug and above
- **Contents:** All logs including diagnostics
- **Retention:** 3 days
- **Use:** Troubleshooting, root cause analysis

**Example Output:**
```
2025-01-24 14:33:02.789 +01:00 [DBG] Agrovet.Application.Services.OrderService Validating product SKU ABC123 against inventory
2025-01-24 14:33:02.890 +01:00 [INF] Creating order for customer 42
2025-01-24 14:33:03.012 +01:00 [INF] Order 108 created successfully
```

---

## HTTP Request Logging

Requests are automatically logged by level:

| Status | Level | Logged | Reason |
|--------|-------|--------|--------|
| 2xx | **Information** | ? | Success |
| 4xx | **Warning** | ? | Client error |
| 5xx | **Error** | ? | Server error |
| Exception | **Error** | ? | Unexpected failure |

**Example:**
```
2025-01-24 14:33:02.123 +01:00 [INF] HTTP POST /api/orders completed in 145.23 ms with status code 201
2025-01-24 14:33:15.456 +01:00 [WRN] HTTP POST /api/orders completed in 50.12 ms with status code 400
2025-01-24 14:33:22.789 +01:00 [ERR] HTTP POST /api/orders completed in 123.45 ms with status code 500
```

---

## Best Practices

### ? Use Structured Logging
```csharp
// Good - enables filtering and analysis
_logger.LogInformation(
    "Product {ProductId} inventory updated from {OldQuantity} to {NewQuantity}",
    product.Id,
    oldQty,
    newQty
);
```

### ? Include Context
```csharp
// Good - helps with debugging
_logger.LogError(
    ex,
    "Payment processing failed for order {OrderId}, customer {CustomerId}",
    orderId,
    customerId
);
```

### ? Use Named Parameters
```csharp
// Good - searchable and readable
_logger.LogInformation("Order {OrderId} shipped to {Address}", order.Id, order.ShippingAddress);

// Bad - no parameters
_logger.LogInformation($"Order {order.Id} shipped to {order.ShippingAddress}");
```

### ? Avoid Logging Secrets
```csharp
// Bad - exposes API key
_logger.LogInformation("Using API key: {ApiKey}", apiKey);

// Good - generic message
_logger.LogInformation("Using configured API endpoint");
```

---

## Logging Dependency Injection

Add logger to services:

```csharp
builder.Services.AddLogging();

// Or specifically for a service
var logger = loggerFactory.CreateLogger<OrderService>();
```

---

## Rules Summary

| Rule | Applies To | Consequence |
|------|-----------|------------|
| No entry/exit logs | All code | Code review rejection |
| Structured logging only | All logs | Code review rejection |
| No sensitive data | All logs | Security review required |
| Business intent only | Info level | Code review feedback |
| Domain layer stays pure | Domain entities | Architecture review |

---

## Review Checklist

Before committing code with logging:

- [ ] Each log has a business purpose
- [ ] No sensitive data (passwords, tokens, keys)
- [ ] Structured parameters used (not string interpolation)
- [ ] Appropriate log level chosen
- [ ] No redundant logs (entry/exit)
- [ ] Exception logs include relevant context
