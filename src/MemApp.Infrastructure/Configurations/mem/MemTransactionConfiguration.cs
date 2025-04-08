using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemTransactionConfiguration : IEntityTypeConfiguration<MemTransaction>
{
    public void Configure(EntityTypeBuilder<MemTransaction> builder)
    {
        builder.ToTable("mem_MemTransaction");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.TransactionNo).IsRequired().HasMaxLength(20);
        builder.Property(a => a.TransActionDate).IsRequired();
        builder.Property(a => a.TransactionMode).IsRequired().HasMaxLength(30);
        builder.Property(a => a.TransactionType).IsRequired().HasMaxLength(20);
        builder.Property(a => a.MemberId).IsRequired();
        builder.Property(a => a.MemberShipNo).IsRequired().HasMaxLength(100);


    }
}
