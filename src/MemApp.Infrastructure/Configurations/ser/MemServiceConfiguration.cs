using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class MemServiceConfiguration : IEntityTypeConfiguration<MemService>
    {
        public void Configure(EntityTypeBuilder<MemService> builder)
        {
            builder.ToTable("mem_MemService");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired();

            builder.HasOne(x => x.ServiceTypes)
                .WithMany(y => y.MemServices)
                .HasForeignKey(z => z.ServiceTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
