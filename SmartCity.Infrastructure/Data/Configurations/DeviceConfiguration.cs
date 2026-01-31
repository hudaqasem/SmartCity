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
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DeviceIdentifier)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasIndex(d => d.DeviceIdentifier).IsUnique();

            builder.Property(d => d.Latitude);
            builder.Property(d => d.Longitude);

            builder.Property(d => d.IsActive).HasDefaultValue(true);

            builder.Property(d => d.LastSeen).IsRequired(false);

            // relationship: owner user
            builder.HasOne(d => d.OwnerUser)
                   .WithMany(u => u.Devices)
                   .HasForeignKey(d => d.OwnerUserId)
                   .OnDelete(DeleteBehavior.SetNull);

            // relationship: subsystem
            builder.HasOne(d => d.Category)
                   .WithMany(s => s.Devices)
                   .HasForeignKey(d => d.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
