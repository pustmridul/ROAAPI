using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class SaleLayoutDetailConfiguration : IEntityTypeConfiguration<SaleLayoutDetail>
    {
        public void Configure(EntityTypeBuilder<SaleLayoutDetail> builder)
        {
            builder.ToTable("mem_SaleLayoutDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.HasOne(x => x.SaleMaster).WithMany(y => y.SaleLayoutDetails).HasForeignKey(z => z.SaleMasterId).OnDelete(DeleteBehavior.NoAction);                  
        }
    }
}
