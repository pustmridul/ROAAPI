using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities;

namespace Res.Infrastructure.Configurations.com;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("mem_User");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.UserName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.AppId).IsRequired().HasMaxLength(50);

        builder.HasIndex(e => e.EmailId).IsUnique();


    }
}
