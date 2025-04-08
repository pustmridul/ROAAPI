using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class MiscItemConfiguration : IEntityTypeConfiguration<MiscItem>
    {
        public void Configure(EntityTypeBuilder<MiscItem> builder)
        {
            builder.ToTable("mem_MiscItem");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
           
        }
    }
}
