using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Data;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class BillDairyRepository : IBillDairy
    {
        private readonly DapperContext context;

        public BillDairyRepository(DapperContext context)
        {
            this.context = context;
        }

        // ================= BILL HISTORY =================

        public async Task<IEnumerable<DTOBillDairy>> GetAllBillHistory(int userId)
        {
            var query = @"
                select * from Bill where user_id=@UserId order by date desc
            ";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    var billHistory = await connection.QueryAsync<DTOBillDairy>(query, new { UserId = userId });
                    return billHistory.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        // ================= BILL IMAGE =================

        public async Task<object> GetBillImageById(long exp_id)
        {
            var query = @"
                select img AS BillImage 
                from Bill 
                where bill_id = @Exp_Id
            ";

            using (var connection = context.CreateConnection())
            {
                var bill = await connection.QuerySingleOrDefaultAsync<DTOBillDairy>(query, new { Exp_Id = exp_id });

                if (bill == null)
                    return null;

                return new
                {
                    bill.BillImage
                };
            }
        }

        // ================= SAVE BILL =================

        public async Task Save(DTOBillDairy objBill)
        {
            var query = @"
                INSERT INTO Bill (user_id,animal_type,price,date,img)
                VALUES (@UserId, @BillType, @Price, @Dt, @img)
            ";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        UserId = objBill.user_id,
                        BillType = objBill.animal_type,
                        Price = objBill.price,
                        Dt = objBill.date,
                        img = objBill.BillImage
                    });
                }
            }
            catch
            {
                throw;
            }
        }

        // ================= EDIT BILL =================

        public async Task Edit(DTOBillDairy objBill)
        {
            var query = @"
                UPDATE Bill 
                SET 
                    animal_type = @BillType,
                    price = @Price,
                    date = @Dt,
                    img = @img
                WHERE bill_id = @Id AND user_id = @UserId
            ";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        Id = objBill.bill_id,
                        UserId = objBill.user_id,
                        BillType = objBill.animal_type,
                        Price = objBill.price,
                        Dt = objBill.date,
                        img = objBill.BillImage
                    });
                }
            }
            catch
            {
                throw;
            }
        }

        // ================= DELETE BILL =================

        public async Task Delete(long exp_id)
        {
            var query = @"
                DELETE FROM Bill 
                WHERE bill_id = @Id
            ";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        Id = exp_id
                    });
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
