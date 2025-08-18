using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;
using System.Numerics;
using Lab_Mvc.Contest;

namespace Lab_Mvc.Repositries
{
    public class EmployeeRepository : IEmployee
    {
        private readonly DapperContext context;

        public EmployeeRepository(DapperContext context)
        {
            //_dbContext = dBContext;
            this.context = context;
        }
        public async Task<IEnumerable<DTOEmployee>> GetEmployees(int comId)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetEmployees);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var Employees = await connection.QueryAsync<DTOEmployee>(query, parameters);
                    return Employees.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DTOEmployee> GetEmployeeById(long emp_code, int comId)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetEmployeeById);
                parameters.Add("@EMP_ID", emp_code);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var Employees = await connection.QuerySingleAsync<DTOEmployee>(query, parameters);
                    return Employees;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveEmployee(DTOEmployee emp)
        {
            try
            {
                var query = "sp_master";


                Int64 newEmployeeId = await GenerateEmployeeId(emp.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertEmployee);
                parameters.Add("@EMP_ID", newEmployeeId);
                parameters.Add("@EMP_NAME", emp.EMP_NAME);
                parameters.Add("@EMP_CONTACT", emp.EMP_CONTACT);
                parameters.Add("@EMP_PASSWORD", emp.EMP_PASSWORD);
                parameters.Add("@COM_ID", emp.COM_ID);



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

        public async Task EditEmployee(DTOEmployee emp, long emp_code)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateEmployee);
                parameters.Add("@EMP_ID", emp_code);
                parameters.Add("@EMP_NAME", emp.EMP_NAME);
                parameters.Add("@EMP_CONTACT", emp.EMP_CONTACT);
                parameters.Add("@EMP_PASSWORD", emp.EMP_PASSWORD);



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

        public async Task DeleteEmployee(long emp_code, int comId)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteEmployee);
                parameters.Add("@EMP_ID", emp_code);
                parameters.Add("@COM_ID", comId);


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

        private async Task<long> GenerateEmployeeId(int comId)
        {
            string fixedPart = "4";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 EMP_ID FROM MST_EMPLOYEE WHERE EMP_ID LIKE @likePattern ORDER BY EMP_ID DESC";

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

                long newEmployeeId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newEmployeeId;
            }
        }



    }
}
