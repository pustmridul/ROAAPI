using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
namespace MemApp.Infrastructure.Configurations.com;

public class UserConferenceConfiguration : IEntityTypeConfiguration<UserConference>
{
    public void Configure(EntityTypeBuilder<UserConference> builder)
    {
        builder.ToTable("mem_UserConference");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.UserName).IsRequired();
    }
}
