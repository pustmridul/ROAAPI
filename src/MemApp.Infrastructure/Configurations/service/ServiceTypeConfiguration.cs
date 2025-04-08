using MemApp.Domain.Entities.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("mem_MemberServiceType");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Title).IsRequired().HasMaxLength(250);
        }
    }
}
