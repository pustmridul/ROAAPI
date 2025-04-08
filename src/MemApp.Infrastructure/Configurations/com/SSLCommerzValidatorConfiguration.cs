using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class SSLCommerzValidatorConfiguration : IEntityTypeConfiguration<SSLCommerzValidator>
{
    public void Configure(EntityTypeBuilder<SSLCommerzValidator> builder)
    {
        builder.ToTable("SSLCommerzValidator");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();      
    }
}
