using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class ServiceTicketDetailConfiguration : IEntityTypeConfiguration<ServiceTicketDetail>
    {
        public void Configure(EntityTypeBuilder<ServiceTicketDetail> builder)
        {
            builder.ToTable("mem_ServiceTicketDetail");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TicketType).IsRequired();

            builder.HasOne(k => k.ServiceTicket)
                 .WithMany(k => k.ServiceTicketDetails)
                 .HasForeignKey(k => new {k.ServiceTicketId })
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(k => k.ServiceTicketType)
               .WithMany(k => k.ServiceTicketDetails)
               .HasForeignKey(k => new { k.ServiceTicketTypeId })
               .OnDelete(DeleteBehavior.NoAction);


        }
    }

    public class EventTokenConfiguration : IEntityTypeConfiguration<EventToken>
    {
        public void Configure(EntityTypeBuilder<EventToken> builder)
        {
            builder.ToTable("mem_EventToken");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TokenCode).IsRequired().HasMaxLength(50);

            builder.HasOne(k => k.ServiceTicket)
                 .WithMany(k => k.EventTokens)
                 .HasForeignKey(k => new { k.ServiceTicketId })
                 .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
