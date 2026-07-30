using Dapper;
using Ecommerce.Application.Repositories;
using Microsoft.Extensions.Configuration;
using ShopEase.Domain.Models.Lookup;
using System.Data;

namespace ShopEase.Application.Repositories
{
    #region Interface
    public interface ILookupRepository
    {
        Task<LookupList> GetRoles(int tenantId);
    }
    #endregion
    public class LookupRepository : BaseRepository, ILookupRepository
    {
        #region Constructor
        public LookupRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region GetRoles
        public async Task<LookupList> GetRoles(int tenantId)
        {
            var data = new LookupList();
            using (var dbConnection = CreateConnection())
            {
                var dynamicParameter = new DynamicParameters();
                dynamicParameter.Add("@TenantId", tenantId);
                dbConnection.Open();
                var result = await dbConnection.QueryMultipleAsync("[dbo].[lkp_Roles]", dynamicParameter, commandType: CommandType.StoredProcedure);
                data.LookupData = result.Read<Lookup>().ToList();
            }
            return data;
        }
        #endregion
    }
}