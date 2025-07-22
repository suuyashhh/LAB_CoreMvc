using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class EmployeeSalaryRepository : IEmployeeSalary
    {
        private readonly DapperContext context;

        public EmployeeSalaryRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOEmployeeSalary>> GetEmployeeSalary(int comId)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetEmployeeSalary);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var EmployeeSalary = await connection.QueryAsync<DTOEmployeeSalary>(query, parameters);
                    return EmployeeSalary.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<DTOEmployeeSalary> GetEmployeeSalaryById(long empSal_id)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetEmployeeSalaryById);
                parameters.Add("@EMP_TRN_ID", empSal_id);

                using (var connection = context.CreateConnection())
                {
                    var EmployeeSalary = await connection.QuerySingleAsync<DTOEmployeeSalary>(query, parameters);
                    return EmployeeSalary;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DTOEmployeeSalary>> GetDateWiseEmpSalary(string from_date, string to_date)
        {
            try
            {
                const string query = "sp_master";
                using (var connection = context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", QueryConstant.GetDateWiseEmpSalary);
                    parameters.Add("@From_Date", from_date);
                    parameters.Add("@To_Date", to_date);

                    using (var multi = await connection.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure))
                    {
                        var casepapers = (await multi.ReadAsync<DTOEmployeeSalary>()).ToList();
                        return casepapers;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching case paper data", ex);
            }
        }

        public async Task SaveEmployeeSalary(DTOEmployeeSalary objEmpSlry)
        {
            try
            {
                var query = "sp_master";


                Int64 newEmployeeSalaryId = await GenerateEmployeeSalaryId(objEmpSlry.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertEmployeeSalary);
                parameters.Add("@EMP_TRN_ID", newEmployeeSalaryId);
                parameters.Add("@EMP_ID", objEmpSlry.EMP_ID);
                parameters.Add("@EMP_PRICE", objEmpSlry.EMP_PRICE);
                parameters.Add("@DATE", objEmpSlry.DATE);
                parameters.Add("@COM_ID", objEmpSlry.COM_ID);
                parameters.Add("@CRT_BY", objEmpSlry.CRT_BY);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task EditEmployeeSalary(DTOEmployeeSalary objEmpSlry, long empSal_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateEmployeeSalary);
                parameters.Add("@EMP_TRN_ID", empSal_id);
                parameters.Add("@EMP_ID", objEmpSlry.EMP_ID);
                parameters.Add("@EMP_PRICE", objEmpSlry.EMP_PRICE);
                parameters.Add("@DATE", objEmpSlry.DATE);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteEmployeeSalary(long empSal_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteEmployeeSalary);
                parameters.Add("@EMP_TRN_ID", empSal_id);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<long> GenerateEmployeeSalaryId(string comId)
        {
            string fixedPart = "203";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 EMP_TRN_ID FROM MST_EMP_SALARY_SLIP WHERE EMP_TRN_ID LIKE @likePattern ORDER BY EMP_TRN_ID DESC";

            using (var connection = context.CreateConnection())
            {
                string lastId = await connection.ExecuteScalarAsync<string>(query, new { likePattern });

                int nextNumber = 1;
                if (!string.IsNullOrEmpty(lastId) && lastId.StartsWith(fixedPart + fixedPartSec))
                {
                    int prefixLength = (fixedPart + fixedPartSec).Length;
                    int lastNumber = int.Parse(lastId.Substring(prefixLength));
                    nextNumber = lastNumber + 1;
                }

                long newEmployeeSalaryId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newEmployeeSalaryId;
            }
        }
    }
}
