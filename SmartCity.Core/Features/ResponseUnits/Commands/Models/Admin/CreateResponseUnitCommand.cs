using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin
{
    public class CreateResponseUnitCommand : IRequest<Response<string>>
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Contact { get; set; } = "";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // account details for the unit
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
