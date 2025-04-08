using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Views;

namespace MemApp.Infrastructure.Configurations.Views;

public class ViewMemberConfiguration : IEntityTypeConfiguration<ViewMember>
{
    public void Configure(EntityTypeBuilder<ViewMember> builder)
    {
        builder.ToView("View_Member");
        builder.HasNoKey();
    }
}
