using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.com;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("mem_User");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.UserName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.AppId).IsRequired().HasMaxLength(50);


    }
}
