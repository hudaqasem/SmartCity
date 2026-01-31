using AutoMapper;

namespace SmartCity.AppCore.Mapping.ResponseUnits
{
    public partial class ResponseUnitProfile : Profile
    {
        public ResponseUnitProfile()
        {
            GetResponsUnitListMapping();
            EditUnitMapping();
        }
    }
}
