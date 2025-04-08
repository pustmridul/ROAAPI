using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class MiscSaleConfiguration : IEntityTypeConfiguration<MiscSale>
    {
        public void Configure(EntityTypeBuilder<MiscSale> builder)
        {
            builder.ToTable("mem_MiscSale");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            
            builder.HasOne(x => x.RegisterMember)
                .WithMany(y => y.MiscSale)
                .HasForeignKey(z => z.MemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class MiscSaleDetailConfiguration : IEntityTypeConfiguration<MiscSaleDetail>
    {
        public void Configure(EntityTypeBuilder<MiscSaleDetail> builder)
        {
            builder.ToTable("mem_MiscSaleDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.MiscSale)
                .WithMany(y => y.MiscSaleDetails)
                .HasForeignKey(z => z.MiscSaleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MiscItem)
               .WithMany(y => y.MiscSaleDetails)
               .HasForeignKey(z => z.ItemId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
