using Amazon.Runtime;
using AutoMapper;
using Ecommerce.Application.DTOs.Response;
using Ecommerce.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Response;
using ShopEase.Application.DTOs.Response.Role;
using ShopEase.Domain.Models;

namespace Ecommerce.Application.Services
{
    #region IRoleService
    public interface IRoleService
    {
        Task<RoleResponseList> GetAllAsync(SortWithPageParameters sortWithPageParameters, int tenantId);
        Task<RoleResponse?> GetByIdAsync(int roleId);
        Task<GenericSaveResponse> SaveAsync(RoleRequest roleRequest, int tenantId, int userId);
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
        public async Task<RoleResponseList> GetAllAsync(
            SortWithPageParameters sortWithPageParameters,
            int tenantId)
        {
            var request = await _roleRepository.GetAll(sortWithPageParameters, tenantId);
            var response = _mapper.Map<RoleList, RoleResponseList>(request);
            return response;
        }
        #endregion

        #region GetByIdAsync
        public async Task<RoleResponse?> GetByIdAsync(int roleId)
        {
            var request = await _roleRepository.GetById(roleId);

            return _mapper.Map<RoleResponse>(request);
        }
        #endregion

        #region SaveAsync
        public async Task<GenericSaveResponse> SaveAsync(RoleRequest roleRequest, int tenantId, int userId)
        {
            var request = _mapper.Map<RoleRequest, Role>(roleRequest);
            var response = await _roleRepository.Save(request, tenantId, userId);
            var result = _mapper.Map<SaveResponse, GenericSaveResponse>(response);
            return result;
        }
        #endregion

        #region DeleteAsync
        public async Task<ApiResponse> DeleteAsync(
            int roleId,
            int deletedBy)
        {
            var response = await _roleRepository.Delete(roleId, deletedBy);

            return _mapper.Map<ApiResponse>(response);
        }
        #endregion
    }
}