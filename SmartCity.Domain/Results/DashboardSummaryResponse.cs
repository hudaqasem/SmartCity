namespace SmartCity.Domain.Results
{
    public class DashboardSummaryResponse
    {
        // Incidents Statistics
        public int TotalIncidents { get; set; }
        public int ActiveIncidents { get; set; }
        public int WaitingForUnit { get; set; }
        public int InProgressIncidents { get; set; }
        public int ResolvedToday { get; set; }

        // Units Statistics
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
        public int BusyUnits { get; set; }
        public int OfflineUnits { get; set; }

        // Performance Metrics
        public double AverageResponseTime { get; set; } // in minutes
        public double AverageCompletionTime { get; set; } // in minutes

        // By Type Breakdown
        public List<IncidentTypeStats> IncidentsByType { get; set; }
        public List<UnitTypeStats> UnitsByType { get; set; }
    }

    public class IncidentTypeStats
    {
        public string Type { get; set; }
        public int Total { get; set; }
        public int Active { get; set; }
        public int Resolved { get; set; }
    }

    public class UnitTypeStats
    {
        public string Type { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public int Busy { get; set; }
    }
}
