using Dapper;
using Lab_Mvc.Contest;
using System.Data;

namespace SmartParking.Repositories
{
    public class DapperRepositoryBase

    {
        protected readonly DapperContext DapperContext;

        protected DapperRepositoryBase(DapperContext dapperContext)
        {
            DapperContext = dapperContext;
        }

        protected IDbConnection CreateConnection()
        {
            return DapperContext.CreateConnection();
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, param);
        }

        protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
        }

        protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, param);
        }

        protected async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<T>(sql, param);
        }
    }
}
