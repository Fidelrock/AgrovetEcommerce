using Agrovet.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Agrovet.Infrastructure.Data.Seed
{
    public static class DbSeeder
    {
    public static async Task SeedAsync(AgrovetDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new("Animal Feeds", "animal-feeds"),
            new("Veterinary Drugs", "veterinary-drugs"),
            new("Farm Equipment", "farm-equipment"),
            new("Seeds & Fertilizers", "seeds-fertilizers")
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new(
                name: "Dairy Meal 50kg",
                description: "High quality dairy meal for milk production",
                sku: "DM-50",
                price: 4200,
                stockQuantity: 100,
                categoryId: categories[0].Id),
            new(
                name: "Newcastle Vaccine",
                description: "Effective poultry vaccine",
                sku: "NV-001",
                price: 850,
                stockQuantity: 50,
                categoryId: categories[1].Id),
            new(
                name: "Knapsack Sprayer",
                description: "16L manual farm sprayer",
                sku: "KS-16L",
                price: 3500,
                stockQuantity: 25,
                categoryId: categories[2].Id)
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
    }
}
