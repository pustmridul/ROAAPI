using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemLedgerConfiguration : IEntityTypeConfiguration<MemLedger>
{
    public void Configure(EntityTypeBuilder<MemLedger> builder)
    {
        builder.ToTable("CustomerLedger");
        builder.HasKey(e => new { e.CustomerLedgerID });
        builder.Property(e => e.CustomerLedgerID).ValueGeneratedOnAdd();
        builder.Property(e => e.CustomerLedgerID).HasColumnType("numeric(18, 0)");
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");

        builder.Property(a => a.PrvCusID).IsRequired().HasMaxLength(10);

    }
}
