using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.Restaurant;

namespace MemApp.Infrastructure.Configurations.Restarunat;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");

        builder.HasKey(r => r.ID);

        builder.Property(r => r.ID).HasColumnName("ID").ValueGeneratedOnAdd();
        builder.Property(r => r.DepartmentName).HasColumnName("DepartmentName");
        builder.Property(r => r.CreatedBy).HasColumnName("CreatedBy");
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(r => r.UpdatedBy).HasColumnName("UpdatedBy");
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate");
        builder.Property(r => r.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(r => r.PrinterName).HasColumnName("PrinterName");

    }

}
