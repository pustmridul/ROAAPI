using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class ServiceTicketConfiguration : IEntityTypeConfiguration<ServiceTicket>
    {
        public void Configure(EntityTypeBuilder<ServiceTicket> builder)
        {
            builder.ToTable("mem_ServiceTicket");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.ServiceChargePercent).IsRequired();
            builder.Property(x => x.ServiceChargeAmount).IsRequired();
            builder.Property(x => x.VatChargePercent).IsRequired();
            builder.Property(x => x.VatChargeAmount).IsRequired();
            builder.HasOne(k => k.MemServices)
                 .WithMany(k => k.ServiceTickets)
                 .HasForeignKey(k => new { k.MemServiceId })
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
