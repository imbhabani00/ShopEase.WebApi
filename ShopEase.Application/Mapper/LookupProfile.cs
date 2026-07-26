using AutoMapper;
using ShopEase.Application.DTOs.Response.Lookup;
using ShopEase.Domain.Models.Lookup;
namespace ShopEase.Application.Mapper
{
    public class LookupProfile : Profile
    {
        public LookupProfile()
        {
            CreateMap<Lookup, LookupResponse>();
            CreateMap<LookupList, LookupResponseList>();
        }
    }
}