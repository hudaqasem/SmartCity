using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Commands.Models;
using SmartCity.AppCore.Features.Incidents.Commands.Results;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Domain.Results;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Incidents.Commands.Handlers
{
    public class IncidentCommandHandler : ResponseHandler,
        IRequestHandler<CreateIncidentCommand, Response<CreateIncidentResponse>>,
        IRequestHandler<CreateIncidentWithAICommand, Response<CreateIncidentAIResponse>>,
        IRequestHandler<RetryAssignCommand, Response<string>>,
        IRequestHandler<RetryAllWaitingCommand, Response<int>>
    {
        private readonly IIncidentService _incidentService;
        private readonly IUnitAssignmentService _unitAssignmentService;
        private readonly IAIDetectionService _aiDetectionService;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentCommandHandler> _logger;

        public IncidentCommandHandler(
            IIncidentService incidentService,
            IUnitAssignmentService unitAssignmentService,
            IAIDetectionService aiDetectionService,
            IMapper mapper,
            ILogger<IncidentCommandHandler> logger)
        {
            _incidentService = incidentService;
            _unitAssignmentService = unitAssignmentService;
            _aiDetectionService = aiDetectionService;
            _mapper = mapper;
            _logger = logger;
        }

        #region Original Create Incident (Without AI) - Manual Report by Citizen

        public async Task<Response<CreateIncidentResponse>> Handle(
            CreateIncidentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Map Command to Entity
                var incident = _mapper.Map<Incident>(request);

                // 1) Create Incident
                var result = await _incidentService.ReportIncidentAsync(incident);

                // 2) Auto Assign Nearest ResponseUnit
                await _unitAssignmentService.AutoAssignUnitAsync(result);

                // 3) Refetch incident with assignments and unit details
                var updatedIncident = await _incidentService.GetByIdAsync(result.Id);

                // 4) Map Entity to Response DTO
                var responseDto = _mapper.Map<CreateIncidentResponse>(updatedIncident ?? result);

                return Created(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating incident: {ex.Message}");
                return BadRequest<CreateIncidentResponse>($"Failed to create incident: {ex.Message}");
            }
        }

        #endregion

        #region Create Incident With AI - Camera/Sensor/AI-Powered Detection

        public async Task<Response<CreateIncidentAIResponse>> Handle(
            CreateIncidentWithAICommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                AIDetectionResponse? aiImageResult = null;
                SOSClassificationResponse? aiTextResult = null;

                // ═══════════════════════════════════════════════════════
                // STEP 1: AI Image Detection (if image provided)
                // ═══════════════════════════════════════════════════════
                if (request.EnableAIDetection && request.ImageData != null && request.ImageData.Length > 0)
                {
                    _logger.LogInformation($"Processing image for {request.Type} detection");

                    try
                    {
                        if (request.Type.Equals("Fire", StringComparison.OrdinalIgnoreCase))
                        {
                            aiImageResult = await _aiDetectionService.DetectFireAsync(request.ImageData);
                        }
                        else if (request.Type.Equals("Medical", StringComparison.OrdinalIgnoreCase) ||
                                 request.Type.Equals("Accident", StringComparison.OrdinalIgnoreCase))
                        {
                            aiImageResult = await _aiDetectionService.DetectAccidentAsync(request.ImageData);
                        }

                        // Validate AI detection confidence
                        if (aiImageResult != null)
                        {
                            if (!aiImageResult.Detected)
                            {
                                _logger.LogWarning(
                                    $"AI did not detect {request.Type} in image. Confidence: {aiImageResult.Confidence:P}"
                                );
                                return BadRequest<CreateIncidentAIResponse>(
                                    $"AI Detection: No {request.Type} detected in the provided image. " +
                                    $"Confidence: {aiImageResult.Confidence:P}. " +
                                    $"Please verify the image and try again."
                                );
                            }

                            if (aiImageResult.Confidence < request.MinimumConfidence)
                            {
                                _logger.LogWarning(
                                    $"AI detection confidence too low: {aiImageResult.Confidence:P} < {request.MinimumConfidence:P}"
                                );
                                return BadRequest<CreateIncidentAIResponse>(
                                    $"AI Detection confidence is below threshold. " +
                                    $"Detected: {aiImageResult.Confidence:P}, Required: {request.MinimumConfidence:P}. " +
                                    $"Image quality may be insufficient."
                                );
                            }

                            // Override incident type with AI recommendation
                            _logger.LogInformation(
                                $"AI detected {aiImageResult.RecommendedUnitType} with {aiImageResult.Confidence:P} confidence"
                            );
                            request.Type = aiImageResult.RecommendedUnitType;
                        }
                    }
                    catch (Exception aiEx)
                    {
                        _logger.LogError($"AI Image Detection failed: {aiEx.Message}");
                        // Fallback: continue without AI detection
                        aiImageResult = null;
                    }
                }

                // ═══════════════════════════════════════════════════════
                // STEP 2: AI Text Classification (if description provided)
                // ═══════════════════════════════════════════════════════
                if (request.EnableAIDetection && !string.IsNullOrWhiteSpace(request.Description))
                {
                    _logger.LogInformation("Analyzing incident description for emergency keywords");

                    try
                    {
                        aiTextResult = await _aiDetectionService.ClassifyTextAsync(
                            request.Description,
                            request.Latitude,
                            request.Longitude
                        );

                        if (aiTextResult != null && aiTextResult.IsEmergency)
                        {
                            _logger.LogInformation(
                                $"AI classified text as emergency: {aiTextResult.IncidentType} " +
                                $"(Confidence: {aiTextResult.Confidence:P})"
                            );

                            // Use AI-recommended type if not already set by image detection
                            if (string.IsNullOrEmpty(request.Type) || aiImageResult == null)
                            {
                                request.Type = aiTextResult.RecommendedUnitType;
                            }
                        }
                    }
                    catch (Exception aiEx)
                    {
                        _logger.LogError($"AI Text Classification failed: {aiEx.Message}");
                        // Fallback: continue without text classification
                        aiTextResult = null;
                    }
                }

                // ═══════════════════════════════════════════════════════
                // STEP 3: Create Incident Entity
                // ═══════════════════════════════════════════════════════
                var incident = _mapper.Map<Incident>(request);
                incident.Status = IncidentStatus.New;

                // Enrich description with AI metadata
                if (aiImageResult != null || aiTextResult != null)
                {
                    incident.Description = EnrichDescriptionWithAI(
                        incident.Description,
                        aiImageResult,
                        aiTextResult
                    );
                }

                // Save incident to database
                var createdIncident = await _incidentService.ReportIncidentAsync(incident);

                _logger.LogInformation($"Incident #{createdIncident.Id} created successfully with AI detection");

                // ═══════════════════════════════════════════════════════
                // STEP 4: Auto-Assign Response Unit
                // ═══════════════════════════════════════════════════════
                await _unitAssignmentService.AutoAssignUnitAsync(createdIncident);

                // ═══════════════════════════════════════════════════════
                // STEP 5: Prepare Response
                // ═══════════════════════════════════════════════════════
                var updatedIncident = await _incidentService.GetByIdAsync(createdIncident.Id);
                var response = _mapper.Map<CreateIncidentAIResponse>(updatedIncident ?? createdIncident);

                // Add AI detection metadata to response
                if (aiImageResult != null)
                {
                    response.AIDetection = new AIDetectionInfo
                    {
                        Detected = aiImageResult.Detected,
                        Confidence = aiImageResult.Confidence,
                        AlertLevel = aiImageResult.AlertLevel,
                        ModelUsed = request.Type.Equals("Fire", StringComparison.OrdinalIgnoreCase)
                            ? "fire.pt"
                            : "accident.pt"
                    };
                }

                _logger.LogInformation(
                    $"Incident #{response.Id} created and assigned. AI Detection: " +
                    $"{(response.AIDetection != null ? "Yes" : "No")}"
                );

                return Created(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating incident with AI: {ex.Message}");
                return BadRequest<CreateIncidentAIResponse>(
                    $"Failed to create incident: {ex.Message}"
                );
            }
        }

        #endregion

        #region Retry Assign Commands

        public async Task<Response<string>> Handle(
            RetryAssignCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _unitAssignmentService.RetryAssignAsync(request.IncidentId);

            if (!result)
                return BadRequest<string>("Retry failed or no units available");

            return Success("Assignment retried successfully");
        }

        public async Task<Response<int>> Handle(
            RetryAllWaitingCommand request,
            CancellationToken cancellationToken)
        {
            var count = await _unitAssignmentService.RetryAllWaitingAsync();

            if (count == 0)
                return BadRequest<int>("No incidents were reassigned");

            return Success(count);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Enrich incident description with AI detection metadata
        /// </summary>
        private string EnrichDescriptionWithAI(
            string originalDescription,
            AIDetectionResponse? imageResult,
            SOSClassificationResponse? textResult)
        {
            var enriched = new System.Text.StringBuilder(originalDescription);

            enriched.AppendLine("\n\n═══ AI Analysis ═══");

            if (imageResult != null)
            {
                enriched.AppendLine($"📷 Image Detection: {imageResult.Description}");
                enriched.AppendLine($"   Confidence: {imageResult.Confidence:P}");
                enriched.AppendLine($"   Alert Level: {imageResult.AlertLevel}");
            }

            if (textResult != null && textResult.IsEmergency)
            {
                enriched.AppendLine($"📝 Text Analysis: {textResult.IncidentType} detected");
                enriched.AppendLine($"   Confidence: {textResult.Confidence:P}");
                enriched.AppendLine($"   Keywords: {string.Join(", ", textResult.KeywordsDetected)}");
            }

            return enriched.ToString();
        }

        #endregion
    }
}