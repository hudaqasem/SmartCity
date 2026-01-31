using SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.ResponseUnits
{
    public partial class ResponseUnitProfile
    {
        public void EditUnitMapping()
        {
            CreateMap<EditResponseUnitCommand, ResponseUnit>();

        }
    }
}

