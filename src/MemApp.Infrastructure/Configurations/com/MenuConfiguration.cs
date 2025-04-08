using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
namespace MemApp.Infrastructure.Configurations.com;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("mem_Menu");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.Name).IsRequired();
        builder.Property(a => a.Url);
        builder.Property(a => a.NavIcon).IsRequired();

        builder.HasOne(e => e.Parent)
           .WithMany(s => s.Childs)
           .HasForeignKey(e => e.ParentId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}
