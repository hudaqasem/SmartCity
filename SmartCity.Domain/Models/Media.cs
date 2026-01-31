using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models
{
    public class Media
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }  // Image, Video
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        //*************** Relations

        public int IncidentId { get; set; }
        public Incident? Incident { get; set; }
    }
}
