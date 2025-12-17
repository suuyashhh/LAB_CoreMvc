using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class DoctorDairyRepository : IDoctorDairy
    {
        private readonly DapperContext context;

        public DoctorDairyRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTODoctorDairy>> GetAllMedicienHistory(int userId)
        {

            var query = @"
                            select * from Expense where expense_name='Medicine' AND user_id=@UserId order by date desc
                          ";
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var MedicienHistorys = await connection.QueryAsync<DTODoctorDairy>(query, new { UserId = userId });

                    return MedicienHistorys.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<object> GetMedicienImageById(long exp_id)
        {
            var query = @"    select img AS AnimalImage 
                              from Expense
                               where expense_id = @Exp_Id
                        ";

            using (var connection = context.CreateConnection())
            {
                var Medicien = await connection.QuerySingleOrDefaultAsync<DTODoctorDairy>(query, new { Exp_Id = exp_id });

                if (Medicien == null)
                {
                    return null;
                }
                return new
                {
                    Medicien.AnimalImage
                };
            }
        }

        public async Task<IEnumerable<DTODoctorDairy>> GetAllDoctorHistory(int userId)
        {

            var query = @"
select * from Expense where expense_name='Doctor' AND user_id=@UserId order by date desc


                          ";
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var FeedsHistorys = await connection.QueryAsync<DTODoctorDairy>(query, new { UserId = userId });

                    return FeedsHistorys.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<object> GetDocImageById(long exp_id)
        {
            var query = @"    select a.animal_image AS AnimalImage 
                              from Expense AS e 
                              JOIN AnimalsName AS a 
                                  ON a.user_id = e.user_id 
                                  AND a.animal_id = e.Animal_id 
                               where e.expense_id = @Exp_Id
                        ";

            using (var connection = context.CreateConnection())
            {
                var Doctor = await connection.QuerySingleOrDefaultAsync<DTODoctorDairy>(query, new { Exp_Id = exp_id });

                if (Doctor == null)
                {
                    return null;
                }
                return new
                {
                    Doctor.AnimalImage
                };
            }
        }


        public async Task Save(DTODoctorDairy objDDoc)
        {
            var query = @"
        INSERT INTO Expense (user_id, expense_name, reason, animal_name, price, date, Animal_id, Switch,img) 
        VALUES (@UserId, @ExpName, @Reasion,@AnimalName, @Price, @Dt, @AnimalId, @Switch,@img)";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        UserId = objDDoc.user_id,
                        ExpName = objDDoc.expense_name,
                        Reasion = objDDoc.reason,
                        AnimalName = objDDoc.animal_name,
                        Price = objDDoc.price,
                        Dt = objDDoc.date,
                        AnimalId = objDDoc.Animal_id,
                        Switch = objDDoc.Switch,
                        img = objDDoc.AnimalImage,
                    });
                }
            }
            catch
            {
                throw;
            }
        }


        public async Task Edit(DTODoctorDairy objDDoc)
        {
            var query = @"
                            UPDATE Expense 
                            SET
                                reason = @Reasion,
                                animal_name = @AnimalName,
                                price = @Price,
                                date = @Dt,
                                Animal_id=@AnimalId,
                                Switch=@Switch,
                                img=@img
                            WHERE expense_id = @Id AND user_id = @UserId";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        Id = objDDoc.expense_id,
                        UserId = objDDoc.user_id,
                        Reasion = objDDoc.reason,
                        AnimalName = objDDoc.animal_name,
                        Price = objDDoc.price,
                        Dt = objDDoc.date,
                        AnimalId = objDDoc.Animal_id,
                        Switch = objDDoc.Switch,
                        img = objDDoc.AnimalImage
                    });
                }
            }
            catch
            {
                throw;
            }
        }


        public async Task Delete(long exp_id)
        {
            var query = @"
    DELETE FROM Expense 
    WHERE expense_id = @Id";

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
