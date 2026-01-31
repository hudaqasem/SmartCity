using SmartCity.Domain.Enums;

namespace SmartCity.Domain.Models
{
    public class ResponseUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Contact { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public UnitStatus Status { get; set; } = UnitStatus.Available;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //*************** Relations 

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Assignment>? Assignments { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }


    }
}
