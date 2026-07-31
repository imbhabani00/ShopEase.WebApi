using AutoMapper;
using Ecommerce.Application.Repositories;
using ShopEase.Application.DTOs.Response;
using ShopEase.Application.DTOs.Response.Customer;
using ShopEase.Application.Repositories;
using ShopEase.Domain.Models;
using ShopEase.Domain.Models.Customer;

namespace ShopEase.Application.Services
{
    public interface ICustomerService
    {
        Task<CustomerResponseList> GetListAsync(SortWithPageParameters sortWithPageParameters, int tenantId);
        Task<GenericSaveResponse> DeleteAsync(int customerId);
        Task<GenericSaveResponse> ActiveInactiveAsync(int customerId, bool isActive);
    }
    public class CustomerService : ICustomerService
    {

        #region Properties
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public CustomerService(
                ICustomerRepository customerRepository,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        #endregion

        #region GetListAsync
        public async Task<CustomerResponseList> GetListAsync(SortWithPageParameters sortWithPageParameters, int tenantId)
        {
            var request = await _customerRepository.GetList(sortWithPageParameters, tenantId);
            var response = _mapper.Map<CustomerList, CustomerResponseList>(request);
            return response;
        }
        #endregion


        #region DeleteAsync
        public async Task<GenericSaveResponse> DeleteAsync(int customerId)
        {
            var request = await _customerRepository.Delete(customerId);
            var response = _mapper.Map<SaveResponse, GenericSaveResponse>(request);
            return response;
        }
        #endregion

        #region ActiveInactive
        public async Task<GenericSaveResponse> ActiveInactiveAsync(int customerId, bool isActive)
        {
            var request = await _customerRepository.ActiveInactive(customerId, isActive);
            var response = _mapper.Map<SaveResponse, GenericSaveResponse>(request);
            return response;
        }
        #endregion
    }
}
