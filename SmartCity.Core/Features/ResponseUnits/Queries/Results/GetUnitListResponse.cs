using SmartCity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.AppCore.Features.ResponseUnits.Queries.Results
{
    public class GetUnitListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Contact { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
