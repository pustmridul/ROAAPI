using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class AddOnsPriceHistoryConfiguration : IEntityTypeConfiguration<AddOnsPriceHistory>
{
    public void Configure(EntityTypeBuilder<AddOnsPriceHistory> builder)
    {
        builder.ToTable("mem_AddOnsPriceHistory");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.Price).IsRequired();
    }
}
