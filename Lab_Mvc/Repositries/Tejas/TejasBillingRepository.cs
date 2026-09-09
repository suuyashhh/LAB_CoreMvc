using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Tejas;
using Models.Tejas;
using SmartParking.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Tejas
{
    public class TejasBillingRepository : DapperRepositoryBase, ITejasBilling
    {
        public TejasBillingRepository(DapperContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TejasBill>> GetAllBills(long? shopId = null)
        {
            var query = @"
                SELECT b.[Id], b.[BillNumber], b.[Subtotal], b.[GrandTotal], b.[CreatedAt], b.[UpdatedAt], b.[TEJAS_SHOPES_ID], s.[SHOP_NAME] AS [ShopName],
                       i.[Id], i.[BillId], i.[FoodId], i.[Name], i.[Price], i.[Quantity], i.[Image]
                FROM [dbo].[Tejas_Bill] b
                LEFT JOIN [dbo].[Tejas_Shopes] s ON b.[TEJAS_SHOPES_ID] = s.[TEJAS_SHOPES_ID]
                LEFT JOIN [dbo].[Tejas_BillItem] i ON b.[Id] = i.[BillId]
                WHERE (@ShopId IS NULL OR b.[TEJAS_SHOPES_ID] = @ShopId)
                ORDER BY b.[CreatedAt] DESC";

            using (var connection = CreateConnection())
            {
                var billDictionary = new Dictionary<string, TejasBill>();

                var list = await connection.QueryAsync<TejasBill, TejasBillItem, TejasBill>(
                    query,
                    (bill, item) =>
                    {
                        if (!billDictionary.TryGetValue(bill.Id, out var billEntry))
                        {
                            billEntry = bill;
                            billEntry.Items = new List<TejasBillItem>();
                            billDictionary.Add(billEntry.Id, billEntry);
                        }

                        if (item != null)
                        {
                            billEntry.Items.Add(item);
                        }
                        return billEntry;
                    },
                    new { ShopId = shopId },
                    splitOn: "Id"
                );

                return billDictionary.Values.ToList();
            }
        }

        public async Task<IEnumerable<TejasBill>> GetBillsByDateRange(System.DateTime startDate, System.DateTime endDate, long? shopId = null)
        {
            var query = @"
                SELECT b.[Id], b.[BillNumber], b.[Subtotal], b.[GrandTotal], b.[CreatedAt], b.[UpdatedAt], b.[TEJAS_SHOPES_ID], s.[SHOP_NAME] AS [ShopName],
                       i.[Id], i.[BillId], i.[FoodId], i.[Name], i.[Price], i.[Quantity], i.[Image]
                FROM [dbo].[Tejas_Bill] b
                LEFT JOIN [dbo].[Tejas_Shopes] s ON b.[TEJAS_SHOPES_ID] = s.[TEJAS_SHOPES_ID]
                LEFT JOIN [dbo].[Tejas_BillItem] i ON b.[Id] = i.[BillId]
                WHERE b.[CreatedAt] >= @StartDate AND b.[CreatedAt] <= @EndDate
                  AND (@ShopId IS NULL OR b.[TEJAS_SHOPES_ID] = @ShopId)
                ORDER BY b.[CreatedAt] DESC";

            using (var connection = CreateConnection())
            {
                var billDictionary = new Dictionary<string, TejasBill>();

                var list = await connection.QueryAsync<TejasBill, TejasBillItem, TejasBill>(
                    query,
                    (bill, item) =>
                    {
                        if (!billDictionary.TryGetValue(bill.Id, out var billEntry))
                        {
                            billEntry = bill;
                            billEntry.Items = new List<TejasBillItem>();
                            billDictionary.Add(billEntry.Id, billEntry);
                        }

                        if (item != null)
                        {
                            billEntry.Items.Add(item);
                        }
                        return billEntry;
                    },
                    new { StartDate = startDate, EndDate = endDate, ShopId = shopId },
                    splitOn: "Id"
                );

                return billDictionary.Values.ToList();
            }
        }

        public async Task<TejasBill?> GetBillById(string id)
        {
            var query = @"
                SELECT b.[Id], b.[BillNumber], b.[Subtotal], b.[GrandTotal], b.[CreatedAt], b.[UpdatedAt], b.[TEJAS_SHOPES_ID], s.[SHOP_NAME] AS [ShopName],
                       i.[Id], i.[BillId], i.[FoodId], i.[Name], i.[Price], i.[Quantity], i.[Image]
                FROM [dbo].[Tejas_Bill] b
                LEFT JOIN [dbo].[Tejas_Shopes] s ON b.[TEJAS_SHOPES_ID] = s.[TEJAS_SHOPES_ID]
                LEFT JOIN [dbo].[Tejas_BillItem] i ON b.[Id] = i.[BillId]
                WHERE b.[Id] = @Id";

            using (var connection = CreateConnection())
            {
                var billDictionary = new Dictionary<string, TejasBill>();

                var list = await connection.QueryAsync<TejasBill, TejasBillItem, TejasBill>(
                    query,
                    (bill, item) =>
                    {
                        if (!billDictionary.TryGetValue(bill.Id, out var billEntry))
                        {
                            billEntry = bill;
                            billEntry.Items = new List<TejasBillItem>();
                            billDictionary.Add(billEntry.Id, billEntry);
                        }

                        if (item != null)
                        {
                            billEntry.Items.Add(item);
                        }
                        return billEntry;
                    },
                    new { Id = id },
                    splitOn: "Id"
                );

                return billDictionary.Values.FirstOrDefault();
            }
        }

        public async Task<string> InsertBill(TejasBill bill)
        {
            var insertBillQuery = @"
                INSERT INTO [dbo].[Tejas_Bill] ([Id], [BillNumber], [Subtotal], [GrandTotal], [CreatedAt], [UpdatedAt], [TEJAS_SHOPES_ID])
                VALUES (@Id, @BillNumber, @Subtotal, @GrandTotal, @CreatedAt, @UpdatedAt, @TEJAS_SHOPES_ID)";

            var insertItemQuery = @"
                INSERT INTO [dbo].[Tejas_BillItem] ([BillId], [FoodId], [Name], [Price], [Quantity])
                VALUES (@BillId, @FoodId, @Name, @Price, @Quantity)";

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync(insertBillQuery, bill, transaction);

                    if (bill.Items != null && bill.Items.Any())
                    {
                        foreach (var item in bill.Items)
                        {
                            item.BillId = bill.Id;
                            await connection.ExecuteAsync(insertItemQuery, item, transaction);
                        }
                    }

                    transaction.Commit();
                    return bill.Id;
                }
            }
        }

        public async Task<int> UpdateBill(TejasBill bill)
        {
            var updateBillQuery = @"
                UPDATE [dbo].[Tejas_Bill]
                SET [Subtotal] = @Subtotal,
                    [GrandTotal] = @GrandTotal,
                    [UpdatedAt] = @UpdatedAt,
                    [TEJAS_SHOPES_ID] = @TEJAS_SHOPES_ID
                WHERE [Id] = @Id";

            var deleteItemsQuery = @"DELETE FROM [dbo].[Tejas_BillItem] WHERE [BillId] = @Id";
            
            var insertItemQuery = @"
                INSERT INTO [dbo].[Tejas_BillItem] ([BillId], [FoodId], [Name], [Price], [Quantity])
                VALUES (@BillId, @FoodId, @Name, @Price, @Quantity)";

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var affected = await connection.ExecuteAsync(updateBillQuery, bill, transaction);

                    await connection.ExecuteAsync(deleteItemsQuery, new { Id = bill.Id }, transaction);

                    if (bill.Items != null && bill.Items.Any())
                    {
                        foreach (var item in bill.Items)
                        {
                            item.BillId = bill.Id;
                            await connection.ExecuteAsync(insertItemQuery, item, transaction);
                        }
                    }

                    transaction.Commit();
                    return affected;
                }
            }
        }

        public async Task<int> DeleteBill(string id)
        {
            // Cascade delete will handle items if configured in DB, but let's be safe
            var deleteItemsQuery = @"DELETE FROM [dbo].[Tejas_BillItem] WHERE [BillId] = @Id";
            var deleteBillQuery = @"DELETE FROM [dbo].[Tejas_Bill] WHERE [Id] = @Id";

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync(deleteItemsQuery, new { Id = id }, transaction);
                    var affected = await connection.ExecuteAsync(deleteBillQuery, new { Id = id }, transaction);
                    transaction.Commit();
                    return affected;
                }
            }
        }

        public async Task<string> GetNextBillNumber(long? shopId = null)
        {
            if (shopId.HasValue && shopId.Value > 1)
            {
                var prefix = $"BR{shopId}-";
                var query = @"
                    SELECT MAX(TRY_CAST(SUBSTRING([BillNumber], LEN(@Prefix) + 1, LEN([BillNumber])) AS INT))
                    FROM [dbo].[Tejas_Bill]
                    WHERE [BillNumber] LIKE @PrefixPattern";

                using (var connection = CreateConnection())
                {
                    var maxId = await connection.ExecuteScalarAsync<int?>(query, new { Prefix = prefix, PrefixPattern = prefix + "%" }) ?? 0;
                    return prefix + (maxId + 1);
                }
            }
            else
            {
                var query = @"
                    SELECT MAX(TRY_CAST(SUBSTRING([BillNumber], 3, LEN([BillNumber]) - 2) AS INT))
                    FROM [dbo].[Tejas_Bill]
                    WHERE [BillNumber] LIKE 'BR%' AND [BillNumber] NOT LIKE 'BR%-%'";

                using (var connection = CreateConnection())
                {
                    var maxId = await connection.ExecuteScalarAsync<int?>(query) ?? 0;
                    return "BR" + (maxId + 1);
                }
            }
        }
    }
}
