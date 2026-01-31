using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Api.Base;
using SmartCity.AppCore.Features.Incidents.Commands.Models;
using SmartCity.AppCore.Features.Incidents.Queries.Models;
using SmartCity.Service.Abstracts;
using System.Security.Claims;

namespace SmartCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncidentController : AppControllerBase
    {
        private string GetCurrentUserId()
        {
            return User.FindFirst("Id")?.Value ??
                   User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }



        #region Citizen
        [Authorize(Roles = "Citizen")]
        [HttpPost]
        public async Task<IActionResult> Report([FromBody] CreateIncidentCommand command)
        {
            var userId = GetCurrentUserId();
            command.ReportedByUserId = userId;
            return NewResult(await Mediator.Send(command));
        }

        [Authorize(Roles = "Citizen")]
        [HttpGet("my-incidents")]
        public async Task<IActionResult> GetMyIncidents()
        {
            var userId = GetCurrentUserId();
            var response = await Mediator.Send(new GetMyIncidentsQuery { UserId = userId });
            return NewResult(response);

        }


        #endregion

        #region Admin

        [Authorize(Roles = "Admin")]
        [HttpGet("List")]
        public async Task<IActionResult> GetAllIncidents()
        {
            var response = await Mediator.Send(new GetIncidentQuery());
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RetryAssign/{id}")]
        public async Task<IActionResult> RetryAssign(int id)
        {
            var response = await Mediator.Send(new RetryAssignCommand(id));
            return NewResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RetryAssignAll")]
        public async Task<IActionResult> RetryAssignAll()
        {
            var response = await Mediator.Send(new RetryAllWaitingCommand());
            return NewResult(response);
        }

        // Filterd Not added to My PostMan collection 
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllIncidentsQuery query)
        {
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        #endregion


        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncidentDetails(int id)
        {
            var response = await Mediator.Send(new GetIncidentByIdQuery { Id = id });
            return NewResult(response);
        }

        #region AI
        /// <summary>
        /// Report incident with AI detection and classification
        /// Supports image upload for fire/accident detection and text analysis for SOS
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/incident/with-ai
        ///     Content-Type: multipart/form-data
        ///     
        ///     {
        ///       "Type": "Fire",
        ///       "Description": "Building fire on 5th floor",
        ///       "Latitude": 30.0444,
        ///       "Longitude": 31.2357,
        ///       "Image": [file upload],
        ///       "EnableAIDetection": true,
        ///       "MinimumConfidence": 0.7
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Incident created successfully with AI validation</response>
        /// <response code="400">AI detection failed or confidence too low</response>

        [Authorize(Roles = "Citizen")]
        [HttpPost("with-ai")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReportWithAI([FromForm] CreateIncidentWithAIDto dto)
        {
            var userId = GetCurrentUserId();

            // Convert uploaded image to byte array
            byte[]? imageData = null;
            if (dto.Image != null && dto.Image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await dto.Image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }

            var command = new CreateIncidentWithAICommand
            {
                Type = dto.Type,
                Description = dto.Description,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ReportedByUserId = userId,
                ImageData = imageData,
                EnableAIDetection = dto.EnableAIDetection,
                MinimumConfidence = dto.MinimumConfidence
            };

            return NewResult(await Mediator.Send(command));
        }
        #endregion

        #region AI Service Health Check

        /// <summary>
        /// Check if AI detection service is running and healthy
        /// </summary>
        [HttpGet("ai-status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAIServiceStatus(
            [FromServices] IAIDetectionService aiService)
        {
            var isHealthy = await aiService.IsAIServiceHealthyAsync();
            return Ok(new
            {
                service = "AI Detection Service",
                status = isHealthy ? "healthy" : "unhealthy",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "/detect/fire",
                    "/detect/accident",
                    "/classify/text"
                }
            });
        }

        #endregion

    }


    /// <summary>
    /// DTO for multipart/form-data requests with image upload
    /// </summary>
    public class CreateIncidentWithAIDto
    {
        /// <summary>
        /// Incident type (Fire, Medical, Police)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Incident description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Location latitude
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Location longitude
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Image file for AI detection
        /// </summary>
        public IFormFile? Image { get; set; }

        /// <summary>
        /// Enable AI detection (default: true)
        /// </summary>
        public bool EnableAIDetection { get; set; } = true;

        /// <summary>
        /// Minimum AI confidence threshold (0.0 to 1.0, default: 0.5)
        /// </summary>
        public float MinimumConfidence { get; set; } = 0.5f;
    }




}
