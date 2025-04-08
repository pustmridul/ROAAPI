using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class MemberTempConfiguration : IEntityTypeConfiguration<MemberTemp>
    {
        public void Configure(EntityTypeBuilder<MemberTemp> builder)
        {
           builder.ToTable("mem_MemberTemp");
           builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();         

            builder.HasOne(x => x.Colleges)
                .WithMany(y=>y.MemberTemps)
                .HasForeignKey(z=>z.CollegeId)
                .OnDelete(DeleteBehavior.NoAction);
           
            builder.HasOne(x => x.MemberProfessions)
                .WithMany(y => y.MemberTemps)
                .HasForeignKey(z => z.MemberProfessionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.BloodGroup)
                .WithMany(y => y.MemberTemps)
                .HasForeignKey(z => z.BloodGroupId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
