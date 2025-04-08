using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class NotificationTokenConfiguration : IEntityTypeConfiguration<NotificationToken>
{
    public void Configure(EntityTypeBuilder<NotificationToken> builder)
    {
        builder.ToTable("com_NotificationToken");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
