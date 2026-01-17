using Agrovet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrovet.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(a => a.AddressLine1)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.AddressLine2)
            .HasMaxLength(500);

        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.State)
            .HasMaxLength(100);

        builder.Property(a => a.PostalCode)
            .HasMaxLength(20);

        builder.Property(a => a.Country)
            .IsRequired()
            .HasMaxLength(100);
    }
}
