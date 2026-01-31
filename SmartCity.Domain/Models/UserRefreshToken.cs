using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCity.Domain.Models
{
    public class UserRefreshToken
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public string RefreshToken { get; set; }
        public string JwtId { get; set; }

        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;

        public DateTime AddedTime { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }

        public ApplicationUser User { get; set; }
    }
}
