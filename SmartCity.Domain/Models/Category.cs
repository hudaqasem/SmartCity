using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //*************** Relations
        public ICollection<Device>? Devices { get; set; }
        public ICollection<Incident>? Incidents { get; set; }
        public ICollection<ResponseUnit>? ResponseUnits { get; set; }
    }
}
