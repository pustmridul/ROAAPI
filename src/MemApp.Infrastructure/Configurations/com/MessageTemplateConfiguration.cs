using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class MessageTemplateConfiguration : IEntityTypeConfiguration<MessageTemplate>
{
    public void Configure(EntityTypeBuilder<MessageTemplate> builder)
    {
        builder.ToTable("com_MessageTemplate");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Name).IsRequired().HasMaxLength(500);

        builder.HasQueryFilter(e => e.IsActive);
    }
}
