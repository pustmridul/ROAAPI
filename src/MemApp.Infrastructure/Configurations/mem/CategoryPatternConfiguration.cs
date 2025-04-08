using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class CategoryPatternConfiguration : IEntityTypeConfiguration<CategoryPattern>
    {
        public void Configure(EntityTypeBuilder<CategoryPattern> builder)
        {
            builder.ToTable("mem_categoryPattern");
            builder.HasKey(x => x.Id);  
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x=>x.Title).IsRequired();

        }
    }
}
