using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class ServiceTicketTypeConfiguration : IEntityTypeConfiguration<ServiceTicketType>
    {
        public void Configure(EntityTypeBuilder<ServiceTicketType> builder)
        {
            builder.ToTable("mem_ServiceTicketType");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Title).IsRequired();
          
          
           // builder.HasOne(x => x.ServiceTicketDetails).WithMany(y => y.ser).HasForeignKey(z => z.ServiceTypeId).OnDelete(DeleteBehavior.NoAction);
            //     builder.HasOne(x => x.MemService).WithMany(y => y.ServiceTicketTypes).HasForeignKey(z => z.MemServiceId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
