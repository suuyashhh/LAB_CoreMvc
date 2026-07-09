using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Market
{
    public class MarketVegitablesRepository : DapperRepositoryBase, IMarketVegitables
    {
        public MarketVegitablesRepository(DapperContext context) : base(context)
        {
        }

       public async Task<List<DTOMarketVegitables>> GetVegetables()
        {
            var query = @"SELECT * FROM Market_Vegitables";
            try
            {
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryAsync<DTOMarketVegitables>(query);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SaveVegitable(string name)
        {
            var query = @"INSERT INTO Market_Vegitables (VegiName)
                  VALUES (@Veginame)";

            try
            {
                using (var connection = CreateConnection())
                {
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        Veginame = name
                    });

                    return rowsAffected > 0 ? "Saved Successfully" : "Insert Failed";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
