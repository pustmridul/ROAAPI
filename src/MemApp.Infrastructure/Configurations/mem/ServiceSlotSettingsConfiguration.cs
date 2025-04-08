using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MemApp.Infrastructure.Configurations.mem
{
    public class ServiceSlotSettingsConfiguration : IEntityTypeConfiguration<ServiceSlotSettings>
    {
        public void Configure(EntityTypeBuilder<ServiceSlotSettings> builder)
        {
            builder.ToTable("mem_ServiceSlotSettings");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
          
            builder.HasOne(k => k.SlotMaster)
            .WithMany(k => k.ServiceSlots)
            .HasForeignKey(k => new { k.SlotMasterId })
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
