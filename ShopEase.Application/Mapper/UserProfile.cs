using AutoMapper;
using Ecommerce.Application.DTOs.Response.User;
using Ecommerce.Domain.Models;
using ShopEase.Application.DTOs.Response;
using ShopEase.Domain.Models;

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
