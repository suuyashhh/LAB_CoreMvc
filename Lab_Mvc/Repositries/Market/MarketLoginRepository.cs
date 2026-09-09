using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Market
{
    public class MarketLoginRepository : DapperRepositoryBase, IMarketLogin
    {
        public MarketLoginRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }
        public async Task<DTOMarketLogin> LoginCheck(string contact, string pass)
        {
            var sql = @"select * from Market_Users Where contact=@Contact AND pass=@pass";

            using var conn = CreateConnection();
            var result = await conn.QuerySingleOrDefaultAsync<DTOMarketLogin>(sql, new { Contact = contact, Pass = pass });
          
            return result;
        }
    }
}
