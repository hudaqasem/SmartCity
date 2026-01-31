namespace SmartCity.Domain.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string DeviceIdentifier { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastSeen { get; set; }

        //*************** Relations
        public string? OwnerUserId { get; set; }
        public ApplicationUser? OwnerUser { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Incident>? Incidents { get; set; }

    }
}
