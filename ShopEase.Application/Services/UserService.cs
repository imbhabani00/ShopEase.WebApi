using AutoMapper;
using Ecommerce.Application.DTOs.Response.User;
using Ecommerce.Application.Repositories;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Http;
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
        Task<UsersResponse> GetByIdAsync(int userId, int tenantId);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiry);
        Task<GenericSaveResponse> SaveAsync(UserRequest userRequest, int tenantId, int loggedInUserId);
        Task<UsersResponseList> GetListAsync(SortWithPageParameters sortWithPageParameters, int tenantId);
        Task<GenericSaveResponse> ChangePasswordAsync(int userId, string passwordHash);
        Task<string?> UploadProfilePictureAsync(int userId, int tenantId, IFormFile file, int modifiedBy);
        Task<bool> RemoveProfilePictureAsync(int userId, int tenantId, int modifiedBy);
        Task<GenericSaveResponse> DeleteAsync(int roleId, int userId);
        Task<GenericSaveResponse> ActiveInactiveAsync(int userId, bool isActive);

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
        public async Task<UsersResponse?> GetByIdAsync(int userId, int tenantId)
        {
            var request = await _userRepository.GetById(userId, tenantId);
            var response = _mapper.Map<Users, UsersResponse>(request);

            if (response != null && !string.IsNullOrEmpty(response.ProfilePicturePath))
            {
                // DB stores the S3 KEY only — resolve it to a browsable URL right here, every time
                response.ProfilePicturePath = await _s3Service.GetDocumentUrl(response.ProfilePicturePath, isPublic: true);
            }
            return response;
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
            var result = _mapper.Map<SaveResponse, GenericSaveResponse>(response);
            return result;
        }
        #endregion

        #region GetListAsync
        public async Task<UsersResponseList> GetListAsync(SortWithPageParameters sortWithPageParameters, int tenantId)
        {
            var request = await _userRepository.GetList(sortWithPageParameters, tenantId);
            var response = _mapper.Map<UsersList, UsersResponseList>(request);
            return response;
        }
        #endregion

        #region DeleteAsync
        public async Task<GenericSaveResponse> DeleteAsync(int roleId, int userId)
        {
            var request = await _userRepository.Delete(roleId, userId);
            var response = _mapper.Map<SaveResponse, GenericSaveResponse>(request);
            return response;
        }
        #endregion

        #region ActiveInactive
        public async Task<GenericSaveResponse> ActiveInactiveAsync(int userId, bool isActive)
        {
            var request = await _userRepository.ActiveInactive(userId , isActive);
            var response = _mapper.Map<SaveResponse, GenericSaveResponse>(request);
            return response;
        }
        #endregion

        #region ChangePasswordAsync
        public async Task<GenericSaveResponse> ChangePasswordAsync(int userId, string passwordHash)
        {
            var request = await _userRepository.ChangePassword(userId, passwordHash);
            var response = _mapper.Map<SaveResponse, GenericSaveResponse>(request);
            return response;
        }
        #endregion

        #region UploadProfilePictureAsync
        public async Task<string?> UploadProfilePictureAsync(int userId, int tenantId, IFormFile file, int modifiedBy)
        {
            using var stream = file.OpenReadStream();

            // returns the S3 KEY e.g. "profile-pictures/5/<guid>_photo.jpg"
            var s3Key = await _s3Service.UploadDocumentAsync(stream, file.FileName, folder: $"profile-pictures/{tenantId}");

            // save the KEY to the DB — never the URL
            await _userRepository.UpdateProfilePicture(userId, tenantId, file.FileName, s3Key, modifiedBy);

            // resolve to a URL just for this response, so the browser can show it immediately
            var publicUrl = await _s3Service.GetDocumentUrl(s3Key, isPublic: true);

            return publicUrl;
        }
        #endregion

        #region RemoveProfilePictureAsync
        public async Task<bool> RemoveProfilePictureAsync(int userId, int tenantId, int modifiedBy)
        {
            var oldPicture = await _userRepository.RemoveProfilePicture(userId, tenantId, modifiedBy);

            if (oldPicture == null || string.IsNullOrEmpty(oldPicture.OldProfilePicturePath))
                return true; // DB cleared fine, nothing was in S3 to delete

            // OldProfilePicturePath holds the KEY — pass straight to delete
            await _s3Service.DeleteDocumentAsync(oldPicture.OldProfilePicturePath);

            return true;
        }
        #endregion
    }
}