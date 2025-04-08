using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemberEducationConfiguration : IEntityTypeConfiguration<MemberEducation>
{
    public void Configure(EntityTypeBuilder<MemberEducation> builder)
    {
        builder.ToTable("mem_Education");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.MemberId).IsRequired();

        builder.HasOne(x => x.MemberTemp)
               .WithMany(y => y.MemberEducations)
               .HasForeignKey(z => z.MemberId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
