namespace SmartCity.Service.Abstracts
{
    public interface IUnitDistanceService
    {
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
