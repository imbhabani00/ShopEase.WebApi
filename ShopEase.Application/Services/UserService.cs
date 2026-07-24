using AutoMapper;
using Ecommerce.Application.DTOs.Response.User;
using Ecommerce.Application.Repositories;
using Ecommerce.Application.Services;
using Microsoft.Extensions.Configuration;
using ShopEase.Application.DTOs.Request.user;
using ShopEase.Application.DTOs.Response;
using ShopEase.Domain.Models;
using ShopEase.Domain.Models.User;

namespace Ecommerce.Service
{
    #region IUserService
    public interface IUserService
    {
        Task<UserGetResponse?> AuthenticateAsync(string email, string password);
        Task<UserResponse?> GetByIdAsync(int userId);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken,DateTime refreshTokenExpiry);
        Task<GenericSaveResponse> SaveAsync(UserRequest userRequest, int tenantId, int loggedInUserId);
        Task<UsersResponseList> GetListAsync(SortWithPageParameters sortWithPageParameters, int tenantId);
    }
    #endregion

    public class UserService : IUserService
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IAWSS3Service _s3Service;
        #endregion

        #region Constructor
        public UserService(
            IUserRepository userRepository,
            IConfiguration config,
            IAWSS3Service s3Service,
            IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _config = config;
            _s3Service = s3Service;
        }
        #endregion

        #region AuthenticateAsync
        public async Task<UserGetResponse?> AuthenticateAsync(string email, string password)
        {
            var request = await _userRepository.Authenticate(email, password);
            var response = _mapper.Map<UserGet, UserGetResponse>(request);
            return response;
        }
        #endregion

        #region GetByIdAsync
        public async Task<UserResponse?> GetByIdAsync(int userId)
        {
            var userEntity = await _userRepository.GetById(userId);
            return _mapper.Map<UserGet, UserResponse>(userEntity);
        }
        #endregion

        #region UpdateRefreshTokenAsync
        public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiry)
        {
            await _userRepository.UpdateRefreshToken(userId, refreshToken, refreshTokenExpiry);
        }
        #endregion

        #region SaveAsync
        public async Task<GenericSaveResponse> SaveAsync(UserRequest userRequest, int tenantId, int loggedInUserId)
        {
            var response = await _userRepository.Save(userRequest, tenantId, loggedInUserId);
            var result = _mapper.Map<SaveResponse, GenericSaveResponse> (response);
            return result;
        }
        #endregion

        #region GetListAsync
        public async Task<UsersResponseList> GetListAsync( SortWithPageParameters sortWithPageParameters,int tenantId)
        {
            var request = await _userRepository.GetList(sortWithPageParameters, tenantId);
            var response = _mapper.Map<UsersList, UsersResponseList>(request);
            return response;
        }
        #endregion
    }
}