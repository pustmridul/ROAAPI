using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.ToTable("CreditCardList");
        builder.HasKey(e => new { e.slno });
        builder.Property(e => e.slno).ValueGeneratedOnAdd();
        builder.Property(e => e.slno).HasColumnType("numeric(18, 0)");

    }
}
