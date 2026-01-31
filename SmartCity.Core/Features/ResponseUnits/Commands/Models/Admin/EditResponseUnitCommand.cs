using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin
{
    public class EditResponseUnitCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Contact { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
