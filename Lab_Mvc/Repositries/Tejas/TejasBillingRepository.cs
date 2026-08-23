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

        public async Task<IEnumerable<TejasBill>> GetAllBills()
        {
            var query = @"
                SELECT b.[Id], b.[BillNumber], b.[Subtotal], b.[GrandTotal], b.[CreatedAt], b.[UpdatedAt],
                       i.[Id], i.[BillId], i.[FoodId], i.[Name], i.[Price], i.[Quantity], i.[Image]
                FROM [dbo].[Tejas_Bill] b
                LEFT JOIN [dbo].[Tejas_BillItem] i ON b.[Id] = i.[BillId]
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
                    splitOn: "Id"
                );

                return billDictionary.Values.ToList();
            }
        }

        public async Task<TejasBill?> GetBillById(string id)
        {
            var query = @"
                SELECT b.[Id], b.[BillNumber], b.[Subtotal], b.[GrandTotal], b.[CreatedAt], b.[UpdatedAt],
                       i.[Id], i.[BillId], i.[FoodId], i.[Name], i.[Price], i.[Quantity], i.[Image]
                FROM [dbo].[Tejas_Bill] b
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
                INSERT INTO [dbo].[Tejas_Bill] ([Id], [BillNumber], [Subtotal], [GrandTotal], [CreatedAt], [UpdatedAt])
                VALUES (@Id, @BillNumber, @Subtotal, @GrandTotal, @CreatedAt, @UpdatedAt)";

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
                    [UpdatedAt] = @UpdatedAt
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

        public async Task<string> GetNextBillNumber()
        {
            var query = "SELECT [BillNumber] FROM [dbo].[Tejas_Bill]";
            using (var connection = CreateConnection())
            {
                var billNumbers = await connection.QueryAsync<string>(query);
                int maxId = 0;
                foreach (var billNumber in billNumbers)
                {
                    if (!string.IsNullOrEmpty(billNumber) && billNumber.StartsWith("BR"))
                    {
                        var numStr = billNumber.Substring(2);
                        if (int.TryParse(numStr, out int num))
                        {
                            if (num > maxId)
                            {
                                maxId = num;
                            }
                        }
                    }
                }
                return "BR" + (maxId + 1);
            }
        }
    }
}
