using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using Dapper;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Market
{
    public class PurchaseRepository : DapperRepositoryBase, IPurchaseRepository
    {
        public PurchaseRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<PurchaseEntry>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    pe.id, 
                    pe.hotel_id AS HotelId, 
                    h.hotel_name AS HotelName, 
                    h.contact_number AS ContactNumber,
                    pe.date, 
                    pe.payment_method AS PaymentMethod, 
                    pe.paid_amount AS PaidAmount, 
                    pe.payment_image AS PaymentImage, 
                    pe.grand_total AS GrandTotal, 
                    pe.notes
                FROM [dbo].[Market_PurchaseEntry] pe
                INNER JOIN [dbo].[Market_Hotel] h ON pe.hotel_id = h.id
                ORDER BY pe.date DESC, pe.id DESC";
            return await connection.QueryAsync<PurchaseEntry>(sql);
        }

        public async Task<PurchaseEntry?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    pe.id, 
                    pe.hotel_id AS HotelId, 
                    h.hotel_name AS HotelName, 
                    h.address AS Address,
                    h.contact_number AS ContactNumber,
                    pe.date, 
                    pe.payment_method AS PaymentMethod, 
                    pe.paid_amount AS PaidAmount, 
                    pe.payment_image AS PaymentImage, 
                    pe.grand_total AS GrandTotal, 
                    pe.notes
                FROM [dbo].[Market_PurchaseEntry] pe
                INNER JOIN [dbo].[Market_Hotel] h ON pe.hotel_id = h.id
                WHERE pe.id = @id;

                SELECT 
                    pi.id, 
                    pi.purchase_id AS PurchaseId, 
                    pi.vegetable_id AS VegetableId, 
                    CASE 
                        WHEN v.Mar_vegetable_name IS NULL OR v.Mar_vegetable_name = '' THEN v.Eng_vegetable_name 
                        ELSE v.Eng_vegetable_name + ' - ' + v.Mar_vegetable_name 
                    END AS VegetableName, 
                    pi.quantity, 
                    pi.price_per_kg AS PricePerKg, 
                    pi.total
                FROM [dbo].[Market_PurchaseItem] pi
                INNER JOIN [dbo].[Market_Vegetable] v ON pi.vegetable_id = v.id
                WHERE pi.purchase_id = @id;";

            using var multi = await connection.QueryMultipleAsync(sql, new { id });
            var entry = await multi.ReadFirstOrDefaultAsync<PurchaseEntry>();
            if (entry != null)
            {
                entry.Items = (await multi.ReadAsync<PurchaseItem>()).ToList();
            }

            return entry;
        }

        public async Task<int> AddAsync(PurchaseEntry entry)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var insertEntrySql = @"
                    INSERT INTO [dbo].[Market_PurchaseEntry] (hotel_id, date, payment_method, paid_amount, payment_image, grand_total, notes)
                    VALUES (@hotel_id, @date, @payment_method, @paid_amount, @payment_image, @grand_total, @notes);
                    SELECT SCOPE_IDENTITY();";

                var newId = await connection.ExecuteScalarAsync<int>(
                    insertEntrySql,
                    new
                    {
                        hotel_id = entry.HotelId,
                        date = entry.Date,
                        payment_method = entry.PaymentMethod,
                        paid_amount = entry.PaidAmount,
                        payment_image = entry.PaymentImage,
                        grand_total = entry.GrandTotal,
                        notes = entry.Notes
                    },
                    transaction: transaction
                );

                var insertItemSql = @"
                    INSERT INTO [dbo].[Market_PurchaseItem] (purchase_id, vegetable_id, quantity, price_per_kg, total)
                    VALUES (@purchase_id, @vegetable_id, @quantity, @price_per_kg, @total);";

                foreach (var item in entry.Items)
                {
                    await connection.ExecuteAsync(
                        insertItemSql,
                        new
                        {
                            purchase_id = newId,
                            vegetable_id = item.VegetableId,
                            quantity = item.Quantity,
                            price_per_kg = item.PricePerKg,
                            total = item.Total
                        },
                        transaction: transaction
                    );
                }

                transaction.Commit();
                return newId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(PurchaseEntry entry)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Update Main Entry
                await connection.ExecuteAsync(
                    @"UPDATE [dbo].[Market_PurchaseEntry]
                      SET hotel_id = @HotelId,
                          date = @Date,
                          payment_method = @PaymentMethod,
                          paid_amount = @PaidAmount,
                          payment_image = @PaymentImage,
                          grand_total = @GrandTotal,
                          notes = @Notes
                      WHERE id = @Id",
                    new
                    {
                        HotelId = entry.HotelId,
                        Date = entry.Date,
                        PaymentMethod = entry.PaymentMethod,
                        PaidAmount = entry.PaidAmount,
                        PaymentImage = entry.PaymentImage,
                        GrandTotal = entry.GrandTotal,
                        Notes = entry.Notes,
                        Id = entry.Id
                    },
                    transaction: transaction
                );

                // Delete Existing Items
                await connection.ExecuteAsync(
                    "DELETE FROM [dbo].[Market_PurchaseItem] WHERE purchase_id = @PurchaseId",
                    new { PurchaseId = entry.Id },
                    transaction: transaction
                );

                // Insert New Items
                var insertItemSql = @"
                    INSERT INTO [dbo].[Market_PurchaseItem] (purchase_id, vegetable_id, quantity, price_per_kg, total)
                    VALUES (@purchase_id, @vegetable_id, @quantity, @price_per_kg, @total);";

                foreach (var item in entry.Items)
                {
                    await connection.ExecuteAsync(
                        insertItemSql,
                        new
                        {
                            purchase_id = entry.Id,
                            vegetable_id = item.VegetableId,
                            quantity = item.Quantity,
                            price_per_kg = item.PricePerKg,
                            total = item.Total
                        },
                        transaction: transaction
                    );
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM [dbo].[Market_PurchaseEntry] WHERE id = @id",
                new { id }
            );
            return rowsAffected > 0;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                DECLARE @TotalHotels INT = (SELECT COUNT(*) FROM [dbo].[Market_Hotel]);
                DECLARE @TotalVegetables INT = (SELECT COUNT(*) FROM [dbo].[Market_Vegetable]);
                DECLARE @TotalPurchases INT = (SELECT COUNT(*) FROM [dbo].[Market_PurchaseEntry]);
                
                DECLARE @TodayStart DATETIME = CAST(GETDATE() AS DATE);
                DECLARE @TodayEnd DATETIME = DATEADD(DAY, 1, @TodayStart);
                
                DECLARE @TodayPurchaseTotal DECIMAL(18,2) = ISNULL((SELECT SUM(grand_total) FROM [dbo].[Market_PurchaseEntry] WHERE date >= @TodayStart AND date < @TodayEnd), 0);
                DECLARE @OverallPurchaseTotal DECIMAL(18,2) = ISNULL((SELECT SUM(grand_total) FROM [dbo].[Market_PurchaseEntry]), 0);

                SELECT 
                    @TotalHotels AS TotalHotels,
                    @TotalVegetables AS TotalVegetables,
                    @TotalPurchases AS TotalPurchaseEntries,
                    @TodayPurchaseTotal AS TodayPurchaseTotal,
                    @OverallPurchaseTotal AS OverallPurchaseTotal;";

            return await connection.QueryFirstOrDefaultAsync<DashboardStats>(sql) ?? new DashboardStats();
        }
    }
}
