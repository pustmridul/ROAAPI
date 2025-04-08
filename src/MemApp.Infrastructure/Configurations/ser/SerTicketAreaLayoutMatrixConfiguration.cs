using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class SerTicketAreaLayoutConfiguration : IEntityTypeConfiguration<SerTicketAreaLayout>
    {
        public void Configure(EntityTypeBuilder<SerTicketAreaLayout> builder)
        {
            builder.ToTable("mem_SerTicketAreaLayout");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
      
            builder.HasOne(k => k.ServiceTicket)
                 .WithMany(k => k.SerTicketAreaLayouts)
                 .HasForeignKey(k => new { k.ServiceTicketId })
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
