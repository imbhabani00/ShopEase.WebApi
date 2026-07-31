using Dapper;
using Ecommerce.Application.Repositories;
using Microsoft.Extensions.Configuration;
using ShopEase.Domain.Models;
using ShopEase.Domain.Models.Customer;
using System.Data;

namespace ShopEase.Application.Repositories
{
    public interface ICustomerRepository
    {
        Task<CustomerList> GetList(SortWithPageParameters sortWithPageParameters, int tenantId);
        Task<SaveResponse> Delete(int customerId);
        Task<SaveResponse> ActiveInactive(int customerId, bool isActive);
    }
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {
        #region Constructor
        public CustomerRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region GetList
        public async Task<CustomerList> GetList(SortWithPageParameters sortWithPageParameters, int tenantId)
        {
            var data = new CustomerList();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenantId", tenantId);
                parameters.Add("@PageNumber", sortWithPageParameters.PageNumber);
                parameters.Add("@PageSize", sortWithPageParameters.PageSize);
                parameters.Add("@SearchString", sortWithPageParameters.SearchString);
                parameters.Add("@SortParameter", sortWithPageParameters.SortParameter);
                parameters.Add("@SortDirection", sortWithPageParameters.SortDirection);

                connection.Open();

                var results = await connection.QueryMultipleAsync(
                    "[dbo].[Customers_GetList]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (data.ReturnValue == 0)
                {
                    data.Customers = results.Read<Customer>().ToList();
                    data.TotalCount = results.ReadFirstOrDefault<int>();
                }
                return data;
            }
        }
        #endregion

        #region Delete
        public async Task<SaveResponse> Delete(int customerId)
        {
            var response = new SaveResponse();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustomerId", customerId);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Open();
                await connection.ExecuteAsync(
                    "[dbo].[Customer_Delete]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                response.ReturnValue = parameters.Get<int>("@ReturnValue");
            }
            return response;
        }
        #endregion

        #region ActiveInactive
        public async Task<SaveResponse> ActiveInactive(int customerId, bool isActive)
        {
            var response = new SaveResponse();
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustomerId", customerId);
                parameters.Add("@IsActive", isActive);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Open();
                await connection.ExecuteAsync(
                    "[dbo].[Customer_Status_Update]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                response.ReturnValue = parameters.Get<int>("@ReturnValue");
            }
            return response;
        }
        #endregion
    }
}
