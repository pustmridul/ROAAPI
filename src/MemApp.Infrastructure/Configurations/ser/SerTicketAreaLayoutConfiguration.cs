using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class SerTicketAreaLayoutMatrixConfiguration : IEntityTypeConfiguration<SerTicketAreaLayoutMatrix>
    {
        public void Configure(EntityTypeBuilder<SerTicketAreaLayoutMatrix> builder)
        {
            builder.ToTable("mem_SerTicketAreaLayoutMatrix");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
      
            builder.HasOne(k => k.SerTicketAreaLayout)
                 .WithMany(k => k.SerTicketAreaLayoutMatrices)
                 .HasForeignKey(k => new { k.SerTicketAreaLayoutId })
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
