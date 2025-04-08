using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Restaurant;

namespace MemApp.Infrastructure.Configurations.Restarunat;

public class RawMeterialGroupConfiguration : IEntityTypeConfiguration<RawMeterialGroup>
{
    public void Configure(EntityTypeBuilder<RawMeterialGroup> builder)
    {
        builder.ToTable("RawMeterialGroup"); 
        builder.HasKey(r => r.ID);
        builder.Property(r => r.ID).HasColumnName("ID"); 
        builder.Property(r => r.GroupID).HasColumnName("GroupID");
        builder.Property(r => r.GroupName).HasColumnName("GroupName");
    }

}
