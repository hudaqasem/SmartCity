using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.Devices.Queries.Models;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : AppControllerBase
    {


        #region GetAll

        [HttpGet("/Device/List")]
        public async Task<IActionResult> GetAllDevices()
        {
            var response = await Mediator.Send(new GetDeviceQuery());
            return Ok(response);
        }

        #endregion

    }
}
