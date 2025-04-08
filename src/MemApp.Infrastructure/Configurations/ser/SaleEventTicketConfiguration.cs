using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class SaleEventTicketConfiguration : IEntityTypeConfiguration<SaleEventTicket>
    {
        public void Configure(EntityTypeBuilder<SaleEventTicket> builder)
        {
            builder.ToTable("mem_SaleEventTicket");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }


    public class SaleEventTicketDetailConfiguration : IEntityTypeConfiguration<SaleEventTicketDetail>
    {
        public void Configure(EntityTypeBuilder<SaleEventTicketDetail> builder)
        {
            builder.ToTable("mem_SaleEventTicketDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.SaleEventTicket)
                .WithMany(y => y.SaleEventTicketDetails)
                .HasForeignKey(z => z.SaleEventId)
                .OnDelete(DeleteBehavior.NoAction);        
        }
    }
}
