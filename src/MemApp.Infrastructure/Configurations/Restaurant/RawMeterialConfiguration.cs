using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.Restaurant;

namespace MemApp.Infrastructure.Configurations.Restarunat;

public class RawMeterialConfiguration : IEntityTypeConfiguration<RawMeterial>
{
    public void Configure(EntityTypeBuilder<RawMeterial> builder)
    {
        builder.ToTable("RawMeterial");

        builder.HasKey(r => r.ID);

        builder.Property(r => r.ID).HasColumnName("ID").ValueGeneratedOnAdd();
        builder.Property(r => r.Name).HasColumnName("Name").HasMaxLength(80).IsRequired();
        builder.Property(r => r.GroupID).HasColumnName("GroupID").IsRequired();
        builder.Property(r => r.UnitTypeID).HasColumnName("UnitTypeID").IsRequired();
        builder.Property(r => r.CPU).HasColumnName("CPU").HasColumnType("money").IsRequired();
        builder.Property(r => r.ROL).HasColumnName("ROL").HasColumnType("money").IsRequired();
        builder.Property(r => r.ReceipeCPU).HasColumnName("ReceipeCPU").HasColumnType("money").IsRequired();
        builder.Property(r => r.ReceipeUnitTypeId).HasColumnName("ReceipeUnitTypeId").IsRequired();
        builder.Property(r => r.ConversionRate).HasColumnName("ConversionRate").HasColumnType("money").IsRequired();
        builder.Property(r => r.IsNonInventoryItem).HasColumnName("IsNonInventoryItem").IsRequired();
        builder.Property(r => r.IsNonExpireItem).HasColumnName("IsNonExpireItem").IsRequired();
        builder.Property(r => r.IsNonReceipyItem).HasColumnName("IsNonReceipyItem").IsRequired();
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(r => r.CreateBy).HasColumnName("CreateBy").HasMaxLength(50).IsRequired();
        builder.Property(r => r.UpdateDate).HasColumnName("UpdateDate");
        builder.Property(r => r.UpdateBy).HasColumnName("UpdateBy").HasMaxLength(50);
    }

}
