using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
{
    public void Configure(EntityTypeBuilder<Reply> builder)
    {
        builder.ToTable("com_Reply");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.HasOne(e => e.Feedback)
          .WithMany(s => s.Replies)
          .HasForeignKey(e => e.FeedbackId)
          .OnDelete(DeleteBehavior.NoAction);
    }
}
