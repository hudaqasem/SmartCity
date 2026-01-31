using AutoMapper;
using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Devices.Queries.Models;
using SmartCity.AppCore.Features.Devices.Queries.Results;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Devices.Queries.Handlers
{
    public class DeviceQueryHandler : ResponseHandler
        , IRequestHandler<GetDeviceQuery, Response<List<GetDeviceListResponse>>>
    {
        #region Fields
        private readonly IDeviceService deviceService;
        private readonly IMapper mapper;

        #endregion

        #region Constructors
        public DeviceQueryHandler(IDeviceService _deviceService,
            IMapper _mapper)
        {
            deviceService = _deviceService;
            mapper = _mapper;
        }

        #endregion

        #region Handle Functions
        public async Task<Response<List<GetDeviceListResponse>>> Handle(GetDeviceQuery request, CancellationToken cancellationToken)
        {
            var deviceList = await deviceService.GetDevicesAsync();
            var deviceListMapper = mapper.Map<List<GetDeviceListResponse>>(deviceList);
            return Success(deviceListMapper);
        }
        #endregion
    }
}
