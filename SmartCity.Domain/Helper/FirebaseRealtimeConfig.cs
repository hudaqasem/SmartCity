using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Helper
{
    public  class FirebaseRealtimeConfig
    {

        public string DatabaseUrl { get; set; } = "";
        public string SensorPath { get; set; } = "sensor_mq2";
        public int PollingSeconds { get; set; } = 3;
        public string ServiceAccountRelativePath { get; set; } = @"smartcity-iot-firebase.json";
    }

}

