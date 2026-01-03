using Agrovet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agrovet.Infrastructure.Data;

public class AgrovetDbContext : DbContext
{
    public AgrovetDbContext(DbContextOptions<AgrovetDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ProductMedia> productMedia => Set<ProductMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgrovetDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category!)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
