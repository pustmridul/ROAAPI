using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.ToTable("BANKLIST");
        builder.HasKey(e => new { e.slno });
        builder.Property(e => e.slno).ValueGeneratedOnAdd();
        builder.Property(e => e.slno).HasColumnType("numeric(18, 0)");


    }
}
