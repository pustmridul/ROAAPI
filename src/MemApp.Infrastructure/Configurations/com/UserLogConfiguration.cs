using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.com;

public class UserLogConfiguration : IEntityTypeConfiguration<UserLog>
{
    public void Configure(EntityTypeBuilder<UserLog> builder)
    {
        builder.ToTable("mem_UserLog");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.UserName).IsRequired().HasMaxLength(100);
    }
}
