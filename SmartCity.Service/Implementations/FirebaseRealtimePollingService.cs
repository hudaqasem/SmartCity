using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartCity.Domain.Helper;
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


        //public FirebaseRealtimePollingService(
        //    ILogger<FirebaseRealtimePollingService> logger,
        //    IOptions<FirebaseRealtimeConfig> cfg)
        //{
        //    _logger = logger;
        //    _cfg = cfg.Value;
        //}
        public FirebaseRealtimePollingService(
            ILogger<FirebaseRealtimePollingService> logger,
            IOptions<FirebaseRealtimeConfig> cfg,
            IHttpClientFactory httpClientFactory,
            IFirebaseAccessTokenProvider tokenProvider)
                {
            _logger = logger;
            _cfg = cfg.Value;
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         
            _logger.LogInformation("🔥 Firebase Polling Service Started. Every {Sec} seconds. Path=/{Path}",
                _cfg.PollingSeconds, _cfg.SensorPath);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("✅ Firebase GET Version Running");

                    var token = await _tokenProvider.GetAccessTokenAsync(stoppingToken);

                    var client = _httpClientFactory.CreateClient("FirebaseRealtime");
                    // تعديل بسيط var url = $"{_cfg.DatabaseUrl.TrimEnd('/')}/{_cfg.SensorPath}.json?access_token={token}";
                    var url = $"{_cfg.DatabaseUrl.TrimEnd('/')}/{_cfg.SensorPath}.json";

                    var sensor = await client.GetFromJsonAsync<SensorData>(url, cancellationToken: stoppingToken);

                    if (sensor == null)
                    {
                        _logger.LogWarning("⚠️ SensorData رجعت null من Firebase");
                    }
                    else
                    {
                        _logger.LogInformation(
                            "📡 SensorData: Status={Status}, Ack={Ack}, Value={Value}, Lat={Lat}, Lng={Lng}",
                            sensor.Status, sensor.Acknowledged, sensor.Value, sensor.Latitude, sensor.Longitude
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error while reading SensorData from Firebase");
                }

                await Task.Delay(TimeSpan.FromSeconds(_cfg.PollingSeconds), stoppingToken);
            }
        }

    }
}

