using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;
using System.Numerics;
using Lab_Mvc.Contest;

namespace Lab_Mvc.Repositries
{
    public class DoctorRepository : IDoctor
    {
        private readonly DapperContext context;

        public DoctorRepository(DapperContext context)
        {
            //_dbContext = dBContext;
            this.context = context;
        }
        public async Task<IEnumerable<DTODoctor>> GetDoctors()
        {
            try
            {
                var query = QueryConstant.GetDoctors;

                using (var connection = context.CreateConnection())
                {
                    var Doctors = await connection.QueryAsync<DTODoctor>(query);
                    return Doctors.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<DTODoctor>> GetDoctorById(long doctor_code)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetDoctorById);
                parameters.Add("@DOCTOR_CODE", doctor_code);

                using (var connection = context.CreateConnection())
                {
                    var Doctors = await connection.QueryAsync<DTODoctor>(query, parameters);
                    return Doctors.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveDoctor(DTODoctor doctor)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertDoctor);
                parameters.Add("@DOCTOR_CODE", doctor.DOCTOR_CODE);
                parameters.Add("@DOCTOR_NAME", doctor.DOCTOR_NAME);
                parameters.Add("@DOCTOR_ADDRESS", doctor.DOCTOR_ADDRESS);
                parameters.Add("@DOCTOR_NUMBER", doctor.DOCTOR_NUMBER);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    //return await property;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task EditDoctor(DTODoctor doctor, long doctor_code)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateDoctor);
                parameters.Add("@DOCTOR_CODE", doctor_code);
                parameters.Add("@DOCTOR_NAME", doctor.DOCTOR_NAME);
                parameters.Add("@DOCTOR_ADDRESS", doctor.DOCTOR_ADDRESS);
                parameters.Add("@DOCTOR_NUMBER", doctor.DOCTOR_NUMBER);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    //return await property;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDoctor(long doctor_code)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteDoctor);
                parameters.Add("@DOCTOR_CODE", doctor_code);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    //return await property;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
