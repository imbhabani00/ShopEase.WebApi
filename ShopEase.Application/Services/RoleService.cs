using Amazon.Runtime;
using AutoMapper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Application.Repositories;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Response.Role;
using ShopEase.Domain.Models;

namespace Ecommerce.Application.Services
{
    #region IRoleService
    public interface IRoleService
    {
        Task<PaginatedResponse<RoleResponse>> GetAllAsync(SortWithPageParameters sortWithPageParameters, int tenantId);
        Task<RoleResponse?> GetByIdAsync(int roleId);
        Task<GenericSaveResponse> CreateAsync(int tenantId, RoleRequest request, int createdBy);
        Task<ApiResponse> UpdateAsync(RoleUpdateRequest request, int modifiedBy);
        Task<ApiResponse> DeleteAsync(int roleId, int deletedBy);
    }
    #endregion

    public class RoleService : IRoleService
    {
        #region Properties
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public RoleService(
            IRoleRepository roleRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        #endregion

        #region GetAllAsync
        public async Task<PaginatedResponse<RoleResponse>> GetAllAsync(
            SortWithPageParameters sortWithPageParameters,
            int tenantId)
        {
            var request = await _roleRepository.GetAllAsync(sortWithPageParameters, tenantId);

            return _mapper.Map<PaginatedResponse<RoleResponse>>(request);
        }
        #endregion

        #region GetByIdAsync
        public async Task<RoleResponse?> GetByIdAsync(int roleId)
        {
            var request = await _roleRepository.GetByIdAsync(roleId);

            return _mapper.Map<RoleResponse>(request);
        }
        #endregion

        #region CreateAsync
        public async Task<GenericSaveResponse> CreateAsync(
            int tenantId,
            GenericSaveResponse request,
            int createdBy)
        {
            var response = await _roleRepository.InsertAsync(tenantId, request, createdBy);

            return _mapper.Map<GenericSaveResponse>(response);
        }
        #endregion

        #region UpdateAsync
        public async Task<ApiResponse> UpdateAsync(
            RoleUpdateRequest request,
            int modifiedBy)
        {
            var response = await _roleRepository.UpdateAsync(request, modifiedBy);

            return _mapper.Map<ApiResponse>(response);
        }
        #endregion

        #region DeleteAsync
        public async Task<ApiResponse> DeleteAsync(
            int roleId,
            int deletedBy)
        {
            var response = await _roleRepository.DeleteAsync(roleId, deletedBy);

            return _mapper.Map<ApiResponse>(response);
        }
        #endregion
    }
}