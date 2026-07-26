using AutoMapper;
using Ecommerce.Application.DTOs.Response.User;
using ShopEase.Application.DTOs.Response;
using ShopEase.Domain.Models;
using ShopEase.Domain.Models.User;

namespace Ecommerce.Application.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User , UserResponse>();
            CreateMap<UserGet, UserGetResponse>();
            CreateMap<SaveResponse, GenericSaveResponse>();
            CreateMap<Users, UsersResponse>();
            CreateMap<UsersList, UsersResponseList>();
        }
    }
}
