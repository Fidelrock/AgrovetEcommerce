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
    public DbSet<ProductMedia> ProductMedia => Set<ProductMedia>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgrovetDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        // Category → Products
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category!)
            .HasForeignKey(p => p.CategoryId);

        // Order → OrderItems (Aggregate Root)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enum persistence (explicit, clean)
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<int>();

        // Order → ShippingAddress
        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
