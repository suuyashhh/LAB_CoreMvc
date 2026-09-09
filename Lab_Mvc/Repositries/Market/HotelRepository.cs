using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using Dapper;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Market
{
    public class HotelRepository : DapperRepositoryBase, IHotelRepository
    {
        public HotelRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            var sql = "SELECT id, hotel_name AS HotelName, address, contact_number AS ContactNumber FROM [dbo].[Market_Hotel] ORDER BY hotel_name";
            return await QueryAsync<Hotel>(sql);
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            var sql = "SELECT id, hotel_name AS HotelName, address, contact_number AS ContactNumber FROM [dbo].[Market_Hotel] WHERE id = @Id";
            return await QuerySingleOrDefaultAsync<Hotel>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Hotel hotel)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "INSERT INTO [dbo].[Market_Hotel] (hotel_name, address, contact_number) VALUES (@hotel_name, @address, @contact_number); SELECT SCOPE_IDENTITY();",
                new { hotel_name = hotel.HotelName, address = hotel.Address, contact_number = hotel.ContactNumber }
            );
        }

        public async Task<bool> UpdateAsync(Hotel hotel)
        {
            using var connection = CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(
                "UPDATE [dbo].[Market_Hotel] SET hotel_name = @hotel_name, address = @address, contact_number = @contact_number WHERE id = @id",
                new { id = hotel.Id, hotel_name = hotel.HotelName, address = hotel.Address, contact_number = hotel.ContactNumber }
            );
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM [dbo].[Market_Hotel] WHERE id = @id",
                new { id }
            );
            return rowsAffected > 0;
        }
    }
}
