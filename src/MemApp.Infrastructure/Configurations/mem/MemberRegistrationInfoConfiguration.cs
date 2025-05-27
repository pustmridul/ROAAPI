using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Res.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemberRegistrationInfoConfiguration : IEntityTypeConfiguration<MemberRegistrationInfo>
{
    public void Configure(EntityTypeBuilder<MemberRegistrationInfo> builder)
    {
        builder.ToTable("MemberRegistrationInfo");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.HasIndex(e => e.MemberNID).IsUnique();
        // builder.Property(a=>a.Title).IsRequired().HasMaxLength(250);

        //builder.HasOne(x => x.User)
        //    .WithMany(y => y.MemberRegistrationInfos)
        //    .HasForeignKey(z => z.UserId)
        //    .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.District)
           .WithMany(y => y.MemberRegistrationInfos)
           .HasForeignKey(z => z.DistrictId)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Division)
             .WithMany(y => y.MemberRegistrationInfos)
             .HasForeignKey(z => z.DivisionId)
             .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Thana)
            .WithMany(y => y.MemberRegistrationInfos)
            .HasForeignKey(z => z.ThanaId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
