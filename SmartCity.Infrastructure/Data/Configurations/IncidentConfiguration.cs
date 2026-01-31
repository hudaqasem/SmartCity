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
    public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Description)
                .HasMaxLength(2000);


            builder.Property(i => i.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // index on Status + CreatedAt for quick filtering
            builder.HasIndex(i => new { i.Status, i.CreatedAt });

            // ReportedByUser relation
            builder.HasOne(i => i.ReportedByUser)
                .WithMany(u => u.Incidents)
                .HasForeignKey(i => i.ReportedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // ReportedByDevice relation
            builder.HasOne(i => i.ReportedByDevice)
                .WithMany(d => d.Incidents)
                .HasForeignKey(i => i.ReportedByDeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Subsystem relation
            builder.HasOne(i => i.Category)
                .WithMany(s => s.Incidents)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
