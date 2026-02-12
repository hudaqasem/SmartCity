using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Helper;
using SmartCity.Domain.Models;
using SmartCity.Domain.Results;
using SmartCity.Service.Abstracts;
using System;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCity.Service.Implementations
{
    public class FirebaseRealtimePollingService : BackgroundService
    {
        private readonly ILogger<FirebaseRealtimePollingService> _logger;
        private readonly FirebaseRealtimeConfig _cfg;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFirebaseAccessTokenProvider _tokenProvider;
        private readonly IServiceScopeFactory _scopeFactory;

        private string? _lastStatus = null;
        private bool? _lastAck = null;

        public FirebaseRealtimePollingService(
            ILogger<FirebaseRealtimePollingService> logger,
            IOptions<FirebaseRealtimeConfig> cfg,
            IHttpClientFactory httpClientFactory,
            IFirebaseAccessTokenProvider tokenProvider,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _cfg = cfg.Value;
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(" Firebase Polling Service Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("FirebaseRealtime");
                    var url = $"{_cfg.DatabaseUrl.TrimEnd('/')}/{_cfg.SensorPath}.json";

                    var sensor = await client.GetFromJsonAsync<SensorData>(url, stoppingToken);

                    if (sensor != null)
                    {
                        if (_lastStatus == null)
                        {
                            _lastStatus = sensor.Status;
                            _lastAck = sensor.Acknowledged;
                            continue;
                        }

                        // اللوجيك: لو خطر والـ Ack لسه false
                        if (sensor.Status.Equals("Danger", StringComparison.OrdinalIgnoreCase) && sensor.Acknowledged == false)
                        {
                            _logger.LogWarning("🚨 FIRE detected! Processing incident...");

                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var incidentService = scope.ServiceProvider.GetRequiredService<IIncidentService>();

                                var incident = new Incident
                                {
                                    Type = "Fire",
                                    Description = sensor.LocationDescription ?? "Fire detected from MQ2 sensor",
                                    Latitude = sensor.Latitude,
                                    Longitude = sensor.Longitude,
                                    Status = IncidentStatus.WaitingForUnit,
                                    CreatedAt = DateTime.Now
                                };

                                var createdIncident = await incidentService.ReportIncidentAsync(incident);
                                _logger.LogInformation("✅ Incident created in SQL. Id = {Id}", createdIncident.Id);
                            }

                            // 1. التعديل الأول: شلنا الـ // عشان يحدث الـ Firebase
                            await SetAcknowledgedAsync(true, stoppingToken);
                        }

                        _lastStatus = sensor.Status;
                        _lastAck = sensor.Acknowledged;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("❌ Error in Firebase Polling: {Msg}", ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(_cfg.PollingSeconds), stoppingToken);
            }
        }

        private async Task SetAcknowledgedAsync(bool value, CancellationToken ct)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FirebaseRealtime");

                // 2. التعديل الثاني: شلنا الـ access_token من الـ URL عشان الـ Rules بتاعتك مفتوحة true حالياً
                var url = $"{_cfg.DatabaseUrl.TrimEnd('/')}/{_cfg.SensorPath}/acknowledged.json";

                using var content = JsonContent.Create(value);
                var resp = await client.PutAsync(url, content, ct);

                if (resp.IsSuccessStatusCode)
                    _logger.LogInformation("✅ Firebase Updated: acknowledged = true");
                else
                    _logger.LogError("⚠️ Firebase Update Failed: {Code}", resp.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Exception in SetAcknowledgedAsync: {Msg}", ex.Message);
            }
        }
    }
}