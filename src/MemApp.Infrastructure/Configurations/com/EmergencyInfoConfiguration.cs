using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class EmergencyInfoConfiguration : IEntityTypeConfiguration<EmergencyInfo>
{
    public void Configure(EntityTypeBuilder<EmergencyInfo> builder)
    {
        builder.ToTable("com_EmergencyInfo");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(500);
        builder.Property(e => e.ContactNo).HasMaxLength(100);

        builder.HasQueryFilter(e => e.IsActive);



    }
}
