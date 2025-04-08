using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class FileInformationConfiguration : IEntityTypeConfiguration<FileInformation>
    {
        public void Configure(EntityTypeBuilder<FileInformation> builder)
        {
            builder.ToTable("mem_FileInformation");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
