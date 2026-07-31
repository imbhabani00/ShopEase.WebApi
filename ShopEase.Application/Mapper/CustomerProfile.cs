using AutoMapper;
using ShopEase.Application.DTOs.Response;
using ShopEase.Application.DTOs.Response.Customer;
using ShopEase.Domain.Models;
using ShopEase.Domain.Models.Customer;

namespace ShopEase.Application.Mapper
{
    public class CustomerProfile :Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerResponse>();
            CreateMap<CustomerList, CustomerResponseList>();
            CreateMap<SaveResponse, GenericSaveResponse>();
        }
    }
}