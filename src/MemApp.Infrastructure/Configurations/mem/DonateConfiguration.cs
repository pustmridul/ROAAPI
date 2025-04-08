using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem;

public class DonateConfiguration : IEntityTypeConfiguration<Donate>
{
    public void Configure(EntityTypeBuilder<Donate> builder)
    {
        builder.ToTable("mem_Donate");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();


        builder.HasOne(k => k.Donation)
             .WithMany(k => k.Donates)
             .HasForeignKey(k => new { k.DonationId })
             .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(k => k.RegisterMember)
            .WithMany(k => k.Donate)
            .HasForeignKey(k => new { k.MemberId })
            .OnDelete(DeleteBehavior.NoAction);

    }
}
