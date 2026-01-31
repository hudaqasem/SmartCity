using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCity.Domain.Models;

namespace SmartCity.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.Property(u => u.FirstName)
                .HasMaxLength(200);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            //// Relationship: User -> Role (many -> one)
            //builder.HasOne(u => u.Role)
            //    .WithMany(r => r.Users)
            //    .HasForeignKey(u => u.RoleId)
            //    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
