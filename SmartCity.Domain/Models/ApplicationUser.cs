using Microsoft.AspNetCore.Identity;

namespace SmartCity.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        //*************** Relations

        public ICollection<Incident>? Incidents { get; set; }
        public ICollection<Device>? Devices { get; set; }
        public ICollection<Notification>? Notifications { get; set; }


        // ********
        public ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new HashSet<UserRefreshToken>();

    }
}
