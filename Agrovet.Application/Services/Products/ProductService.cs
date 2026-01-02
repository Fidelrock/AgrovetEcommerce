using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Agrovet.Application.Services.Products;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Product> CreateAsync(
        string name,
        string description,
        string sku,
        decimal price,
        int stockQuantity,
        Guid categoryId)
    {
        _logger.LogInformation("Creating product {ProductName}", name);

        var product = new Product(
            name,
            description,
            sku,
            price,
            stockQuantity,
            categoryId);

        await _productRepository.AddAsync(product);

        _logger.LogInformation(
            "Product {ProductId} created successfully",
            product.Id);

        return product;
    }
}
