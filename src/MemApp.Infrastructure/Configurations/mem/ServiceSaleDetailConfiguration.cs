using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class ServiceSaleDetailConfiguration : IEntityTypeConfiguration<ServiceSaleDetail>
{
    public void Configure(EntityTypeBuilder<ServiceSaleDetail> builder)
    {
        builder.ToTable("mem_ServiceSaleDetail");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        //builder.Property(a=>a.Name).IsRequired().HasMaxLength(250);

        builder.HasOne(k => k.ServiceSale)
            .WithMany(k => k.ServiceSaleDetails)
            .HasForeignKey(k => new { k.ServiceSaleId })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(k => k.ServiceTicket)
           .WithMany(k => k.ServiceSaleDetails)
           .HasForeignKey(k => new { k.ServiceTicketId })
           .OnDelete(DeleteBehavior.NoAction);
    }
}
