using AutoMapper;
using ShopEase.Application.DTOs.Response.Lookup;
using ShopEase.Application.Repositories;
using ShopEase.Domain.Models.Lookup;

namespace ShopEase.Application.Services
{
    #region Interface
    public interface ILookupService
    {
        Task<LookupResponseList> GetRolesAsync(int tenantId);
    }
    #endregion

    public class LookupService : ILookupService
    {
        #region Properties
        private readonly ILookupRepository _lookupRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public LookupService(ILookupRepository lookupRepository, IMapper mapper)
        {
            _lookupRepository = lookupRepository;
            _mapper = mapper;
        }
        #endregion

        #region GetRoles
        public async Task<LookupResponseList> GetRolesAsync(int tenantId)
        {
            var request = await _lookupRepository.GetRoles(tenantId);
            var response = _mapper.Map<LookupList, LookupResponseList>(request);
            return response;
        }
        #endregion
    }
}