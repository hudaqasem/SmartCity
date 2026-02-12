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
                var incident = _mapper.Map<Incident>(request);
                var result = await _incidentService.ReportIncidentAsync(incident);
                await _unitAssignmentService.AutoAssignUnitAsync(result);
                var updatedIncident = await _incidentService.GetByIdAsync(result.Id);
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

        #region Create Incident With AI - FINAL CORRECTED VERSION

        public async Task<Response<CreateIncidentAIResponse>> Handle(
            CreateIncidentWithAICommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Store all detection results
                var detectionResults = new List<(string ModelName, AIDetectionResponse Result)>();
                SOSClassificationResponse? textClassification = null;

                // ═══════════════════════════════════════════════════════════
                // STEP 1: Run ALL AI Models on the Image
                // ═══════════════════════════════════════════════════════════
                if (request.EnableAIDetection && request.ImageData != null && request.ImageData.Length > 0)
                {
                    _logger.LogInformation("🔍 Running multi-model AI detection on image...");

                    // ✅ Test FIRE Model
                    try
                    {
                        var fireResult = await _aiDetectionService.DetectFireAsync(request.ImageData);

                        _logger.LogInformation(
                            $"🔥 Fire Model: Detected={fireResult.Detected}, " +
                            $"Confidence={fireResult.Confidence:P}, " +
                            $"Type={fireResult.IncidentType}, " +
                            $"RecommendedUnit={fireResult.RecommendedUnitType}"
                        );

                        // ✅ Only store if actually detected
                        if (fireResult.Detected)
                        {
                            detectionResults.Add(("Fire", fireResult));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"⚠️ Fire detection failed: {ex.Message}");
                    }

                    // ✅ Test ACCIDENT Model
                    try
                    {
                        var accidentResult = await _aiDetectionService.DetectAccidentAsync(request.ImageData);

                        _logger.LogInformation(
                            $"🚗 Accident Model: Detected={accidentResult.Detected}, " +
                            $"Confidence={accidentResult.Confidence:P}, " +
                            $"Type={accidentResult.IncidentType}, " +
                            $"RecommendedUnit={accidentResult.RecommendedUnitType}"
                        );

                        // ✅ Only store if actually detected
                        if (accidentResult.Detected)
                        {
                            detectionResults.Add(("Accident", accidentResult));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"⚠️ Accident detection failed: {ex.Message}");
                    }

                    // ═══════════════════════════════════════════════════════════
                    // STEP 2: Select Best Detection (Highest Confidence)
                    // ═══════════════════════════════════════════════════════════
                    if (!detectionResults.Any())
                    {
                        _logger.LogWarning("❌ No incidents detected in image by any AI model");

                        return BadRequest<CreateIncidentAIResponse>(
                            "AI could not detect any emergency situation in the provided image. " +
                            "Please verify the image shows a clear emergency (fire, accident, etc.)"
                        );
                    }

                    // ✅ Sort by confidence and pick the highest
                    var bestDetection = detectionResults
                        .OrderByDescending(x => x.Result.Confidence)
                        .First();

                    _logger.LogInformation(
                        $"🎯 BEST DETECTION: {bestDetection.ModelName} model " +
                        $"(Confidence: {bestDetection.Result.Confidence:P}, " +
                        $"Type: {bestDetection.Result.RecommendedUnitType})"
                    );

                    // ✅ Validate confidence threshold
                    if (bestDetection.Result.Confidence < request.MinimumConfidence)
                    {
                        _logger.LogWarning(
                            $"⚠️ Best detection confidence ({bestDetection.Result.Confidence:P}) " +
                            $"is below threshold ({request.MinimumConfidence:P})"
                        );

                        return BadRequest<CreateIncidentAIResponse>(
                            $"AI Detection confidence is below threshold. " +
                            $"Detected: {bestDetection.Result.RecommendedUnitType} at {bestDetection.Result.Confidence:P}, " +
                            $"Required: {request.MinimumConfidence:P}. " +
                            $"Please provide a clearer image."
                        );
                    }

                    // ═══════════════════════════════════════════════════════════
                    // STEP 3: Optional Text Classification
                    // ═══════════════════════════════════════════════════════════
                    string finalType = bestDetection.Result.RecommendedUnitType;
                    float finalConfidence = bestDetection.Result.Confidence;
                    AIDetectionResponse finalDetection = bestDetection.Result;

                    if (!string.IsNullOrWhiteSpace(request.Description))
                    {
                        _logger.LogInformation("📝 Analyzing incident description...");

                        try
                        {
                            textClassification = await _aiDetectionService.ClassifyTextAsync(
                                request.Description,
                                request.Latitude,
                                request.Longitude
                            );

                            if (textClassification != null && textClassification.IsEmergency)
                            {
                                _logger.LogInformation(
                                    $"📝 Text Classification: Type={textClassification.IncidentType}, " +
                                    $"Confidence={textClassification.Confidence:P}, " +
                                    $"Keywords={string.Join(", ", textClassification.KeywordsDetected)}"
                                );

                                // ✅ Use text ONLY if confidence is significantly higher (>15% difference)
                                if (textClassification.Confidence > (finalConfidence + 0.15f))
                                {
                                    finalType = textClassification.RecommendedUnitType;
                                    finalConfidence = textClassification.Confidence;

                                    _logger.LogInformation(
                                        $"🔄 Overriding image detection with text classification: {finalType}"
                                    );
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"⚠️ Text classification failed: {ex.Message}");
                        }
                    }

                    // ═══════════════════════════════════════════════════════════
                    // STEP 4: Create Incident with AI-Detected Type
                    // ═══════════════════════════════════════════════════════════
                    _logger.LogInformation(
                        $"✅ FINAL DECISION: Type={finalType}, Confidence={finalConfidence:P}"
                    );

                    var incident = _mapper.Map<Incident>(request);

                    // ✅ CRITICAL: Set type from AI detection
                    incident.Type = finalType;
                    incident.Status = IncidentStatus.New;

                    // Enrich description with AI metadata
                    incident.Description = EnrichDescriptionWithAI(
                        incident.Description,
                        finalDetection,
                        textClassification
                    );

                    _logger.LogInformation(
                        $"💾 Creating incident: Type={incident.Type}, Status={incident.Status}"
                    );

                    // Save to database
                    var createdIncident = await _incidentService.ReportIncidentAsync(incident);

                    _logger.LogInformation(
                        $"✅ Incident #{createdIncident.Id} created successfully"
                    );

                    // ═══════════════════════════════════════════════════════════
                    // STEP 5: Auto-Assign Response Unit
                    // ═══════════════════════════════════════════════════════════
                    _logger.LogInformation(
                        $"🚨 Auto-assigning {incident.Type} unit to incident #{createdIncident.Id}"
                    );

                    await _unitAssignmentService.AutoAssignUnitAsync(createdIncident);

                    // ═══════════════════════════════════════════════════════════
                    // STEP 6: Prepare Response
                    // ═══════════════════════════════════════════════════════════
                    var updatedIncident = await _incidentService.GetByIdAsync(createdIncident.Id);
                    var response = _mapper.Map<CreateIncidentAIResponse>(updatedIncident ?? createdIncident);

                    // Add AI detection metadata
                    response.AIDetection = new AIDetectionInfo
                    {
                        Detected = true,
                        Confidence = finalDetection.Confidence,
                        AlertLevel = finalDetection.AlertLevel,
                        ModelUsed = bestDetection.ModelName
                    };

                    _logger.LogInformation(
                        $"✅ SUCCESS! Incident #{response.Id} created and assigned. " +
                        $"Type: {response.Type}, " +
                        $"Confidence: {finalConfidence:P}, " +
                        $"Unit: {response.Assignment?.UnitName ?? "Waiting for assignment"}"
                    );

                    return Created(response);
                }
                else
                {
                    // ═══════════════════════════════════════════════════════════
                    // No Image - Try Text Only
                    // ═══════════════════════════════════════════════════════════
                    if (string.IsNullOrWhiteSpace(request.Description))
                    {
                        return BadRequest<CreateIncidentAIResponse>(
                            "Please provide either an image or a description for AI analysis"
                        );
                    }

                    _logger.LogInformation("📝 No image provided - using text classification only");

                    textClassification = await _aiDetectionService.ClassifyTextAsync(
                        request.Description,
                        request.Latitude,
                        request.Longitude
                    );

                    if (textClassification == null || !textClassification.IsEmergency)
                    {
                        return BadRequest<CreateIncidentAIResponse>(
                            "AI could not detect an emergency situation from the provided description"
                        );
                    }

                    if (textClassification.Confidence < request.MinimumConfidence)
                    {
                        return BadRequest<CreateIncidentAIResponse>(
                            $"Text classification confidence ({textClassification.Confidence:P}) " +
                            $"is below threshold ({request.MinimumConfidence:P})"
                        );
                    }

                    // Create incident with text-detected type
                    var incident = _mapper.Map<Incident>(request);
                    incident.Type = textClassification.RecommendedUnitType;
                    incident.Status = IncidentStatus.New;
                    incident.Description = EnrichDescriptionWithAI(incident.Description, null, textClassification);

                    var createdIncident = await _incidentService.ReportIncidentAsync(incident);
                    await _unitAssignmentService.AutoAssignUnitAsync(createdIncident);

                    var updatedIncident = await _incidentService.GetByIdAsync(createdIncident.Id);
                    var response = _mapper.Map<CreateIncidentAIResponse>(updatedIncident ?? createdIncident);

                    response.AIDetection = new AIDetectionInfo
                    {
                        Detected = true,
                        Confidence = textClassification.Confidence,
                        AlertLevel = textClassification.AlertLevel,
                        ModelUsed = "Text Classification"
                    };

                    return Created(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error creating incident with AI: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");

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

        private string EnrichDescriptionWithAI(
            string originalDescription,
            AIDetectionResponse? imageResult,
            SOSClassificationResponse? textResult)
        {
            var enriched = new System.Text.StringBuilder(originalDescription);

            enriched.AppendLine("\n\n═══ AI Analysis ═══");

            if (imageResult != null)
            {
                enriched.AppendLine($"🔍 Image Detection: {imageResult.Description}");
                enriched.AppendLine($"   Type: {imageResult.IncidentType}");
                enriched.AppendLine($"   Confidence: {imageResult.Confidence:P}");
                enriched.AppendLine($"   Alert Level: {imageResult.AlertLevel}");
            }

            if (textResult != null && textResult.IsEmergency)
            {
                enriched.AppendLine($"📝 Text Analysis: {textResult.IncidentType} emergency detected");
                enriched.AppendLine($"   Confidence: {textResult.Confidence:P}");
                enriched.AppendLine($"   Keywords: {string.Join(", ", textResult.KeywordsDetected)}");
            }

            return enriched.ToString();
        }

        #endregion
    }
}