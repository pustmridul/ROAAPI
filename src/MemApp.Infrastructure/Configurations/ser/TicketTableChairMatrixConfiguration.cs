using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class TicketTableChairMatrixConfiguration : IEntityTypeConfiguration<TicketTableChairMatrix>
    {
        public void Configure(EntityTypeBuilder<TicketTableChairMatrix> builder)
        {
            builder.ToTable("mem_TicketTableChairMatrix");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();         

        }
    }
}
