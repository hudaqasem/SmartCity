using SmartCity.AppCore.Features.ResponseUnits.Queries.Results;
using SmartCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.AppCore.Mapping.ResponseUnits
{
    public partial class ResponseUnitProfile
    {
        public void GetResponsUnitListMapping()
        {
            CreateMap<ResponseUnit, GetUnitListResponse>()
                   .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status));
        }
    }
}
