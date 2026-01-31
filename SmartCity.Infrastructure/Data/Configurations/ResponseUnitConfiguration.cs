using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCity.Domain.Models;

namespace SmartCity.Infrastructure.Data.Configurations
{
    public class ResponseUnitConfiguration : IEntityTypeConfiguration<ResponseUnit>
    {
        public void Configure(EntityTypeBuilder<ResponseUnit> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.Type)
                   .HasMaxLength(100);

            builder.Property(r => r.Contact)
                   .HasMaxLength(200);



            builder.Property(r => r.IsActive).HasDefaultValue(true);
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(r => new { r.Type, r.Status });

            builder.HasOne(r => r.Category)
                   .WithMany(s => s.ResponseUnits)
                   .HasForeignKey(r => r.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(r => r.Assignments)
                   .WithOne(a => a.Unit)
                   .HasForeignKey(a => a.UnitId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
