using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("com_Feedback");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
