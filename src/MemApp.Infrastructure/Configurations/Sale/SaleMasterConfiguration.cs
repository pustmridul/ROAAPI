using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class SaleMasterConfiguration : IEntityTypeConfiguration<SaleMaster>
    {
        public void Configure(EntityTypeBuilder<SaleMaster> builder)
        {
            builder.ToTable("mem_SaleMaster");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.HasOne(x => x.ServiceTypes).WithMany(y => y.SaleMasters).HasForeignKey(z => z.ServiceTypeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.MemServices).WithMany(y => y.SaleMasters).HasForeignKey(z => z.MemServiceId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.ServiceTicket).WithMany(y => y.SaleMasters).HasForeignKey(z => z.ServiceTicketId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
