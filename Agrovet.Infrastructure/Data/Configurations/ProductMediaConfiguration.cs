using Agrovet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrovet.Infrastructure.Data.Configurations;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Url)
            .IsRequired();

        builder.Property(pm => pm.MediaType)
            .IsRequired();

        builder.HasOne(pm => pm.Product)
            .WithMany(p => p.Media)
            .HasForeignKey(pm => pm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
