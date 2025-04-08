using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class RegisterMemberConfiguration : IEntityTypeConfiguration<RegisterMember>
    {
        public void Configure(EntityTypeBuilder<RegisterMember> builder)
        {
            builder.ToTable("Customer");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FullName).IsRequired();


            builder.HasIndex(e => new { e.Id, e.MembershipNo });

            builder.HasOne(x => x.Colleges)
                .WithMany(y => y.RegisterMembers)
                .HasForeignKey(z => z.CollegeId)
                .OnDelete(DeleteBehavior.NoAction);
            //builder.HasOne(x => x.MemberStatus).WithMany(y => y.RegisterMembers).HasForeignKey(z => z.MemberStatusId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.MemberTypes)
                .WithMany(y => y.RegisterMembers)
                .HasForeignKey(z => z.MemberTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MemberProfessions)
                .WithMany(y => y.RegisterMembers)
                .HasForeignKey(z => z.MemberProfessionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.BloodGroup)
                .WithMany(y => y.RegisterMembers)
                .HasForeignKey(z => z.BloodGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MemberActiveStatus)
                .WithMany(y => y.RegisterMembers)
                .HasForeignKey(z => z.MemberActiveStatusId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
