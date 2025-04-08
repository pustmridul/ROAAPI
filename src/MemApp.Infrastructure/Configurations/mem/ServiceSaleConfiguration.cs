using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class ServiceSaleConfiguration : IEntityTypeConfiguration<ServiceSale>
{
    public void Configure(EntityTypeBuilder<ServiceSale> builder)
    {
        builder.ToTable("mem_ServiceSale");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        //builder.Property(a=>a.Name).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.RegisterMember)
               .WithMany(y => y.ServiceSale)
               .HasForeignKey(z => z.MemberId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
