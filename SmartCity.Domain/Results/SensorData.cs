using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartCity.Domain.Results
{
    /// <summary>
    /// Matches the exact JSON structure coming from Firebase Realtime DB under /sensor_mq2
    /// {
    ///   "acknowledged": false,
    ///   "last_triggered": "2025-12-19 02:19:00 PM",
    ///   "latitude": 31.2658,
    ///   "location_description": "Fixed Sensor – Near New Bus Station, Port Said Al-Dawahi",
    ///   "longitude": 32.3019,
    ///   "status": "Normal",
    ///   "value": 253
    /// }
    /// </summary>
    public class SensorData
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty; // "Normal" / "Danger"
        [JsonPropertyName("acknowledged")]
        public bool Acknowledged { get; set; }                       //
        [JsonPropertyName("last_triggered")]                                                           // false = لم يتم الإقرار بعد
        public string LastTriggered { get; set; } = string.Empty;    // آخر مرة تغير الـ status
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("location_description")]
        public string LocationDescription { get; set; } = string.Empty;
    }
}
                     // false = لم يتم الإقرار بعد



