using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;

namespace Lab_Mvc.Repositries.Market
{
    public class MarketLoginRepository : IMarketLogin
    {
        private readonly DapperContext _dapperContext;

        public MarketLoginRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<DTOMarketLogin> LoginCheck(string contact, string pass)
        {
            var sql = @"select * from Market_Users Where contact=@Contact AND pass=@pass";

            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QuerySingleOrDefaultAsync<DTOMarketLogin>(sql, new { Contact = contact, Pass = pass });
          
            return result;
        }
    }
}
