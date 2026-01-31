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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Channel)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(n => n.Payload)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);


            builder.Property(n => n.RetryCount)
                   .HasDefaultValue(0);

            builder.Property(n => n.SentAt).IsRequired(false);

            builder.HasOne(n => n.TargetUser)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.TargetUserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(n => n.Incident)
                   .WithMany(i => i.Notifications)
                   .HasForeignKey(n => n.IncidentId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
