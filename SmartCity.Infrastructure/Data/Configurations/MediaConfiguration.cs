using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Infrastructure.Data.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Url)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(m => m.Type)
                   .HasMaxLength(50);

            builder.Property(m => m.UploadedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(m => m.Incident)
                   .WithMany(i => i.MediaFiles)
                   .HasForeignKey(m => m.IncidentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
