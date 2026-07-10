using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lab_Mvc.Contest;
using Models.Market;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_Mvc.Controllers.Market
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/backup")] // Fallback explicit route mapping
    public class MarketBackupController : ControllerBase
    {
        private readonly DapperContext _dapperContext;

        public MarketBackupController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public class BackupData
        {
            public List<Hotel> Hotels { get; set; } = new();
            public List<Vegetable> Vegetables { get; set; } = new();
            public List<PurchaseEntryExport> PurchaseEntries { get; set; } = new();
            public List<PurchaseItemExport> PurchaseItems { get; set; } = new();
        }

        public class PurchaseEntryExport
        {
            public int Id { get; set; }
            public int HotelId { get; set; }
            public DateTime Date { get; set; }
            public string PaymentMethod { get; set; } = string.Empty;
            public decimal PaidAmount { get; set; }
            public string? PaymentImage { get; set; }
            public decimal GrandTotal { get; set; }
            public string? Notes { get; set; }
        }

        public class PurchaseItemExport
        {
            public int Id { get; set; }
            public int PurchaseId { get; set; }
            public int VegetableId { get; set; }
            public decimal Quantity { get; set; }
            public decimal PricePerKg { get; set; }
            public decimal Total { get; set; }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportData()
        {
            using var connection = _dapperContext.CreateConnection();
            
            var hotels = (await connection.QueryAsync<Hotel>(
                "SELECT id, hotel_name AS HotelName, address, contact_number AS ContactNumber FROM [dbo].[Market_Hotel]"
            )).ToList();

            var vegetables = (await connection.QueryAsync<Vegetable>(
                "SELECT id, vegetable_name AS VegetableName FROM [dbo].[Market_Vegetable]"
            )).ToList();

            var entries = (await connection.QueryAsync<PurchaseEntryExport>(
                "SELECT id, hotel_id AS HotelId, date, payment_method AS PaymentMethod, paid_amount AS PaidAmount, payment_image AS PaymentImage, grand_total AS GrandTotal, notes FROM [dbo].[Market_PurchaseEntry]"
            )).ToList();

            var items = (await connection.QueryAsync<PurchaseItemExport>(
                "SELECT id, purchase_id AS PurchaseId, vegetable_id AS VegetableId, quantity, price_per_kg AS PricePerKg, total FROM [dbo].[Market_PurchaseItem]"
            )).ToList();

            var backup = new BackupData
            {
                Hotels = hotels,
                Vegetables = vegetables,
                PurchaseEntries = entries,
                PurchaseItems = items
            };

            return Ok(backup);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportData([FromBody] BackupData data)
        {
            if (data == null) return BadRequest(new { Message = "Invalid backup data." });

            using var connection = _dapperContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Delete existing records in dependency order
                await connection.ExecuteAsync("DELETE FROM [dbo].[Market_PurchaseItem]", transaction: transaction);
                await connection.ExecuteAsync("DELETE FROM [dbo].[Market_PurchaseEntry]", transaction: transaction);
                await connection.ExecuteAsync("DELETE FROM [dbo].[Market_Hotel]", transaction: transaction);
                await connection.ExecuteAsync("DELETE FROM [dbo].[Market_Vegetable]", transaction: transaction);

                // Insert Vegetables
                if (data.Vegetables.Any())
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_Vegetable] ON", transaction: transaction);
                    foreach (var veg in data.Vegetables)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO [dbo].[Market_Vegetable] (id, vegetable_name) VALUES (@Id, @VegetableName)",
                            new { Id = veg.Id, VegetableName = veg.VegetableName },
                            transaction: transaction
                        );
                    }
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_Vegetable] OFF", transaction: transaction);
                }

                // Insert Hotels
                if (data.Hotels.Any())
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_Hotel] ON", transaction: transaction);
                    foreach (var hotel in data.Hotels)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO [dbo].[Market_Hotel] (id, hotel_name, address, contact_number) VALUES (@Id, @HotelName, @Address, @ContactNumber)",
                            new { Id = hotel.Id, HotelName = hotel.HotelName, Address = hotel.Address, ContactNumber = hotel.ContactNumber },
                            transaction: transaction
                        );
                    }
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_Hotel] OFF", transaction: transaction);
                }

                // Insert PurchaseEntries
                if (data.PurchaseEntries.Any())
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_PurchaseEntry] ON", transaction: transaction);
                    foreach (var pe in data.PurchaseEntries)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO [dbo].[Market_PurchaseEntry] (id, hotel_id, date, payment_method, paid_amount, payment_image, grand_total, notes) VALUES (@Id, @HotelId, @Date, @PaymentMethod, @PaidAmount, @PaymentImage, @GrandTotal, @Notes)",
                            new { Id = pe.Id, HotelId = pe.HotelId, Date = pe.Date, PaymentMethod = pe.PaymentMethod, PaidAmount = pe.PaidAmount, PaymentImage = pe.PaymentImage, GrandTotal = pe.GrandTotal, Notes = pe.Notes },
                            transaction: transaction
                        );
                    }
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_PurchaseEntry] OFF", transaction: transaction);
                }

                // Insert PurchaseItems
                if (data.PurchaseItems.Any())
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_PurchaseItem] ON", transaction: transaction);
                    foreach (var pi in data.PurchaseItems)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO [dbo].[Market_PurchaseItem] (id, purchase_id, vegetable_id, quantity, price_per_kg, total) VALUES (@Id, @PurchaseId, @VegetableId, @Quantity, @PricePerKg, @Total)",
                            new { Id = pi.Id, PurchaseId = pi.PurchaseId, VegetableId = pi.VegetableId, Quantity = pi.Quantity, PricePerKg = pi.PricePerKg, Total = pi.Total },
                            transaction: transaction
                        );
                    }
                    await connection.ExecuteAsync("SET IDENTITY_INSERT [dbo].[Market_PurchaseItem] OFF", transaction: transaction);
                }

                transaction.Commit();
                return Ok(new { Message = "Backup data restored successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, new { Message = "Failed to restore backup data.", Error = ex.Message });
            }
        }
    }
}
