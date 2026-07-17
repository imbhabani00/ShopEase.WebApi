using AutoMapper;
using ShopEase.Application.DTOs.Request;
using ShopEase.Application.DTOs.Response;
using ShopEase.Application.DTOs.Response.Role;
using ShopEase.Domain.Models;

namespace ShopEase.Application.Mapper
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleResponse>();
            CreateMap<RoleList, RoleResponseList>();
            CreateMap<RoleRequest, Role>();
            CreateMap<SaveResponse, GenericSaveResponse>();
        }
    }
}