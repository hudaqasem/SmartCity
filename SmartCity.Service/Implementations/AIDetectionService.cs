using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartCity.Domain.Results;
using SmartCity.Service.Abstracts;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartCity.Service.Implementations
{
    public class AIDetectionService : IAIDetectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _aiApiBaseUrl;
        private readonly ILogger<AIDetectionService> _logger;
        private readonly int _timeoutSeconds;

        public AIDetectionService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AIDetectionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _aiApiBaseUrl = configuration["AIService:BaseUrl"] ?? "http://localhost:5000";
            _timeoutSeconds = configuration.GetValue<int>("AIService:Timeout", 30);

            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        /// <summary>
        /// Detect fire in an image
        /// </summary>
        public async Task<AIDetectionResponse> DetectFireAsync(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
            }

            try
            {
                _logger.LogInformation("Sending fire detection request to AI service");

                using var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageData);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "file", "image.jpg");

                var response = await _httpClient.PostAsync(
                    $"{_aiApiBaseUrl}/detect/fire",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI Fire Detection failed: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException(
                        $"AI service returned {response.StatusCode}. Details: {errorContent}"
                    );
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AIDetectionResponse>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                _logger.LogInformation(
                    $"Fire detection completed. Detected: {result?.Detected}, Confidence: {result?.Confidence:P}"
                );

                return result ?? throw new Exception("Failed to deserialize AI response");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"AI Fire Detection timeout after {_timeoutSeconds}s: {ex.Message}");
                throw new Exception($"AI service request timed out after {_timeoutSeconds} seconds", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"AI Fire Detection HTTP error: {ex.Message}");
                throw new Exception($"Failed to communicate with AI service: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AI Fire Detection unexpected error: {ex.Message}");
                throw new Exception($"AI Fire Detection failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Detect accident in an image
        /// </summary>
        public async Task<AIDetectionResponse> DetectAccidentAsync(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
            }

            try
            {
                _logger.LogInformation("Sending accident detection request to AI service");

                using var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageData);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "file", "image.jpg");

                var response = await _httpClient.PostAsync(
                    $"{_aiApiBaseUrl}/detect/accident",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI Accident Detection failed: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException(
                        $"AI service returned {response.StatusCode}. Details: {errorContent}"
                    );
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AIDetectionResponse>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                _logger.LogInformation(
                    $"Accident detection completed. Detected: {result?.Detected}, Confidence: {result?.Confidence:P}"
                );

                return result ?? throw new Exception("Failed to deserialize AI response");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"AI Accident Detection timeout after {_timeoutSeconds}s: {ex.Message}");
                throw new Exception($"AI service request timed out after {_timeoutSeconds} seconds", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"AI Accident Detection HTTP error: {ex.Message}");
                throw new Exception($"Failed to communicate with AI service: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AI Accident Detection unexpected error: {ex.Message}");
                throw new Exception($"AI Accident Detection failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Classify text for emergency keywords
        /// </summary>
        public async Task<SOSClassificationResponse> ClassifyTextAsync(
            string text,
            double? latitude = null,
            double? longitude = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or empty", nameof(text));
            }

            try
            {
                _logger.LogInformation($"Sending text classification request: '{text.Substring(0, Math.Min(50, text.Length))}...'");

                var requestData = new
                {
                    text = text,
                    latitude = latitude,
                    longitude = longitude
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(
                    jsonContent,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    $"{_aiApiBaseUrl}/classify/text",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI Text Classification failed: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException(
                        $"AI service returned {response.StatusCode}. Details: {errorContent}"
                    );
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SOSClassificationResponse>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                _logger.LogInformation(
                    $"Text classification completed. Emergency: {result?.IsEmergency}, Confidence: {result?.Confidence:P}, Type: {result?.IncidentType}"
                );

                return result ?? throw new Exception("Failed to deserialize AI response");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"AI Text Classification timeout after {_timeoutSeconds}s: {ex.Message}");
                throw new Exception($"AI service request timed out after {_timeoutSeconds} seconds", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"AI Text Classification HTTP error: {ex.Message}");
                throw new Exception($"Failed to communicate with AI service: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AI Text Classification unexpected error: {ex.Message}");
                throw new Exception($"AI Text Classification failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Health check for AI service
        /// </summary>
        public async Task<bool> IsAIServiceHealthyAsync()
        {
            try
            {
                _logger.LogInformation("Checking AI service health");

                var response = await _httpClient.GetAsync($"{_aiApiBaseUrl}/health");

                var isHealthy = response.IsSuccessStatusCode;

                _logger.LogInformation($"AI service health check: {(isHealthy ? "Healthy" : "Unhealthy")}");

                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"AI service health check failed: {ex.Message}");
                return false;
            }
        }
    }
}