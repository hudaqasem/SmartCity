using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Abstracts
{
    public interface IIncidentRepository : IGenericRepositoryAsync<Incident>
    {
        Task<List<Incident>> GetWaitingIncidentsAsync();
        public Task<List<Incident>> GetWaitingForUnitAsync();
    }
}
