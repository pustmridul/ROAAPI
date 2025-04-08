using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class SaleTicketDetailConfiguration : IEntityTypeConfiguration<SaleTicketDetail>
    {
        public void Configure(EntityTypeBuilder<SaleTicketDetail> builder)
        {
            builder.ToTable("mem_SaleTicketDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.SaleMaster)
                .WithMany(y => y.SaleTicketDetails)
                .HasForeignKey(z => z.SaleMasterId)
                .OnDelete(DeleteBehavior.NoAction);        
            builder.HasOne(x => x.ServiceTicketDetail)
                .WithMany(y => y.SaleTicketDetails)
                .HasForeignKey(z => z.ServiceTicketDetailId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
