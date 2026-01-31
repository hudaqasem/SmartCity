using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class ResponseUnitService : IResponseUnitService
    {
        #region Fields

        private readonly IResponseUnitRepository responseUniteRepository;

        #endregion

        #region Constructors
        public ResponseUnitService(IResponseUnitRepository _responseUniteRepository)
        {
            responseUniteRepository = _responseUniteRepository;
        }


        #endregion

        #region Handles Functions

        public async Task<List<ResponseUnit>> GetResponseUnitsAsync()
        {
            return await responseUniteRepository.GetAllAsync();
        }

        public async Task<ResponseUnit?> GetResponseUnitByUserIdAsync(string userId)
        {
            return await responseUniteRepository.GetByUserIdAsync(userId);

        }

        public async Task<ResponseUnit?> GetResponseUnitByIdAsync(int unitId)
        {
            return await responseUniteRepository.GetByIdAsync(unitId);

        }

        #endregion
    }
}
