using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Data.Configurations;

namespace SmartCity.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<ResponseUnit> ResponseUnits { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration)
                .Assembly);


        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //new ProductConfiguration().Configure(modelBuilder.Entity<Product>());
        //    //new CategoryConfiguration().Configure(modelBuilder.Entity<Category>());

        //    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        //}

    }
}
