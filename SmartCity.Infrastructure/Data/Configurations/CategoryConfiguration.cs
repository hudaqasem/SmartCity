using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.Description)
                   .HasMaxLength(1000);

            builder.Property(s => s.IsActive).HasDefaultValue(true);
            builder.Property(s => s.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
