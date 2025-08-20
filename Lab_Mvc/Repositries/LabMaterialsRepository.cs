using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Microsoft.Identity.Client;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class LabMaterialsRepository : ILabMaterials
    {
        private readonly DapperContext context; 

        public LabMaterialsRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOLabMaterials>> GetLabMaterials(int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetLabMaterials);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var labMaterials = await connection.QueryAsync<DTOLabMaterials>(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return labMaterials.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<DTOLabMaterials> GetLabMaterialsById(long mat_id, int comId)
        {
            try
            {
                var query = "dbo.sp_master";
                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetMaterialById);
                parameters.Add("@MAT_ID", mat_id);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var labMaterial = await connection.QuerySingleAsync<DTOLabMaterials>(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return labMaterial;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<DTOLabMaterials>> GetDateWiseLabMaterials(string from_date, string to_date, int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetDateWiseLabMaterials);
                parameters.Add("@From_Date", from_date);
                parameters.Add("@To_Date", to_date);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                using (var multi = await connection.QueryMultipleAsync(
                    query,
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    var labMaterials = (await multi.ReadAsync<DTOLabMaterials>()).ToList();
                    return labMaterials;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching lab materials by date range.", ex);
            }
        }

        public async Task SaveLabMaterials(DTOLabMaterials objMat)
        {
            try
            {
                var query = QueryConstant.sp;

                long newMatId = await GenerateLabMaterialsId(objMat.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertMaterials);
                parameters.Add("@MAT_ID", newMatId);
                parameters.Add("@MAT_NAME", objMat.MAT_NAME);
                parameters.Add("@MAT_PRICE", objMat.MAT_PRICE);
                parameters.Add("@DATE", objMat.DATE);
                parameters.Add("@COM_ID", objMat.COM_ID);
                parameters.Add("@CRT_BY", objMat.CRT_BY);

                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task EditLabMaterials(DTOLabMaterials objMat, long mat_id)
        {
            try
            {
                var query = QueryConstant.sp;

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateMaterials);
                parameters.Add("@MAT_ID", mat_id);
                parameters.Add("@MAT_NAME", objMat.MAT_NAME);
                parameters.Add("@MAT_PRICE", objMat.MAT_PRICE);
                parameters.Add("@DATE", objMat.DATE);

                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteLabMaterials(long mat_id, int comId)
        {
            try
            {
                var query = QueryConstant.sp;

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteMaterials);
                parameters.Add("@MAT_ID", mat_id);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task<long> GenerateLabMaterialsId(string comId)
        {
            string fixedPart = "201";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 MAT_ID FROM MST_MATERIALS WHERE MAT_ID LIKE @likePattern ORDER BY MAT_ID DESC";

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

                long newMatId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newMatId;
            }
        }


    }
}
