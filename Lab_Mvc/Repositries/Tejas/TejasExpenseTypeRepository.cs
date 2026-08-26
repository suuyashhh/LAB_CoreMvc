using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Tejas;
using Models.Tejas;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Tejas
{
    public class TejasExpenseTypeRepository : DapperRepositoryBase, ITejasExpenseType
    {
        public TejasExpenseTypeRepository(DapperContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DTOTejasExpenseType>> GetAll()
        {
            var query = @"
                SELECT 
                    [EX_ID],
                    [NAME]
                FROM [dbo].[SHOP_EXPENSE_TYPE]
                ORDER BY [EX_ID] DESC";

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<DTOTejasExpenseType>(query);
                return result;
            }
        }

        public async Task<DTOTejasExpenseType?> GetById(int exId)
        {
            var query = @"
                SELECT 
                    [EX_ID],
                    [NAME]
                FROM [dbo].[SHOP_EXPENSE_TYPE]
                WHERE [EX_ID] = @ExId";

            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<DTOTejasExpenseType>(
                    query,
                    new { ExId = exId }
                );
                return result;
            }
        }

        public async Task<int> Insert(DTOTejasExpenseType model)
        {
            var query = @"
                INSERT INTO [dbo].[SHOP_EXPENSE_TYPE] ([NAME])
                VALUES (@NAME);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, model);
                return id;
            }
        }

        public async Task<int> Update(DTOTejasExpenseType model)
        {
            var query = @"
                UPDATE [dbo].[SHOP_EXPENSE_TYPE]
                SET [NAME] = @NAME
                WHERE [EX_ID] = @EX_ID";

            using (var connection = CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(int exId)
        {
            var query = @"
                DELETE FROM [dbo].[SHOP_EXPENSE_TYPE]
                WHERE [EX_ID] = @ExId";

            using (var connection = CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new { ExId = exId }
                );
                return affectedRows;
            }
        }
    }
}
