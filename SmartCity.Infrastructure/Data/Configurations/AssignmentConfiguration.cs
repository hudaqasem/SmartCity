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
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {

            builder.HasKey(a => new { a.IncidentId, a.UnitId }); //Composite Primary Key M to M relation


            builder.Property(a => a.AssignedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(a => a.Incident)
                   .WithMany(i => i.Assignments)
                   .HasForeignKey(a => a.IncidentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Unit)
                   .WithMany(u => u.Assignments)
                   .HasForeignKey(a => a.UnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(a => a.DispatcherUser)
            //       .WithMany(u => u.Dispatches)
            //       .HasForeignKey(a => a.DispatcherUserId)
            //       .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
