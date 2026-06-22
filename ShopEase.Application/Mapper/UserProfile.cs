using AutoMapper;
using Ecommerce.Application.DTOs.Response.User;
using Ecommerce.Domain.Models;

namespace Ecommerce.Application.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User , UserResponse>();
            CreateMap<UserGet, UserGetResponse>();
        }
    }
}
