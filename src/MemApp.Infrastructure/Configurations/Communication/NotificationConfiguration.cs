using FirebaseAdmin.Messaging;
using MemApp.Domain.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Configurations.Communication
{
    public class NotificationConfiguration : IEntityTypeConfiguration<AdminNotification>
    {
        public void Configure(EntityTypeBuilder<AdminNotification> builder)
        {
            builder.ToTable("mem_AdminNotification");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
