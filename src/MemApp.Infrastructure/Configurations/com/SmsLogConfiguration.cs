using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.com;

public class SmsLogConfiguration : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> builder)
    {
        builder.ToTable("mem_SmsLog");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
      

    }
}
