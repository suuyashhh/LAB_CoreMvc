using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.BillingApp;
using Models.BillingApp;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.BillingApp
{
    public class ProductEntriesRepository : DapperRepositoryBase, IProductEntries
    {
        public ProductEntriesRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<DTOProductEntries>> GetAllEntries()
        {
            const string query = @"
                SELECT 
                    product_id, 
                    english_name, 
                    marathi_name, 
                    weight, 
                    price, 
                    barcodeNo
                FROM Billing_ProductEntries
                ORDER BY product_id DESC";
            try
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<DTOProductEntries>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveProductEntry(DTOProductEntries entry)
        {
            const string query = @"
                INSERT INTO Billing_ProductEntries 
                (
                    english_name, 
                    marathi_name, 
                    weight, 
                    price, 
                    barcodeNo
                )
                VALUES 
                (
                    @english_name, 
                    @marathi_name, 
                    @weight, 
                    @price, 
                    @barcodeNo
                )";
            try
            {
                using var connection = CreateConnection();
                int affectedRows = await connection.ExecuteAsync(query, entry);
                return affectedRows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateProductEntry(DTOProductEntries entry)
        {
            const string query = @"
                UPDATE Billing_ProductEntries
                SET 
                    english_name = @english_name,
                    marathi_name = @marathi_name,
                    weight = @weight,
                    price = @price,
                    barcodeNo = @barcodeNo
                WHERE product_id = @product_id";
            try
            {
                using var connection = CreateConnection();
                int affectedRows = await connection.ExecuteAsync(query, entry);
                return affectedRows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProductEntry(long productId)
        {
            const string query = @"
                DELETE FROM Billing_ProductEntries
                WHERE product_id = @productId";
            try
            {
                using var connection = CreateConnection();
                int affectedRows = await connection.ExecuteAsync(query, new { productId });
                return affectedRows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
