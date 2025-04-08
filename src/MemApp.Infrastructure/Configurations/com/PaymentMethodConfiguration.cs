using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentTypeManagement");
        builder.HasKey(e => new { e.PaymentTypeID });
        builder.Property(a => a.Name).IsRequired().HasMaxLength(150);

    }
}
