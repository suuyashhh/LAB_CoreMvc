using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class DoctorCommissionRepository : IDoctorCommission
    {
        private readonly DapperContext context;

        public DoctorCommissionRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTODoctorCommission>> GetDoctorCommission()
        {
            try
            {
                var query = QueryConstant.GetDoctorCommission;

                using (var connection = context.CreateConnection())
                {
                    var DoctorCommission = await connection.QueryAsync<DTODoctorCommission>(query);
                    return DoctorCommission.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<DTODoctorCommission> GetDoctorCommissionById(long docCom_id)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetDoctorCommissionById);
                parameters.Add("@DOC_COM_ID", docCom_id);

                using (var connection = context.CreateConnection())
                {
                    var DoctorCommission = await connection.QuerySingleAsync<DTODoctorCommission>(query, parameters);
                    return DoctorCommission;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveDoctorCommission(DTODoctorCommission objDocCom)
        {
            try
            {
                var query = "sp_master";


                Int64 newDoctorCommissionId = await GenerateDoctorCommissionId(objDocCom.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertDoctorCommission);
                parameters.Add("@DOC_COM_ID", newDoctorCommissionId);
                parameters.Add("@DOCTOR_ID", objDocCom.DOCTOR_ID);
                parameters.Add("@DOC_COM_PRICE", objDocCom.DOC_COM_PRICE);
                parameters.Add("@DATE", objDocCom.DATE);
                parameters.Add("@COM_ID", objDocCom.COM_ID);
                parameters.Add("@CRT_BY", objDocCom.CRT_BY);



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

        public async Task EditDoctorCommission(DTODoctorCommission objDocCom, long docCom_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateDoctorCommission);
                parameters.Add("@DOC_COM_ID", docCom_id);
                parameters.Add("@DOCTOR_ID", objDocCom.DOCTOR_ID);
                parameters.Add("@DOC_COM_PRICE", objDocCom.DOC_COM_PRICE);
                parameters.Add("@DATE", objDocCom.DATE);



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

        public async Task DeleteDoctorCommission(long docCom_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteDoctorCommission);
                parameters.Add("@DOC_COM_ID", docCom_id);



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

        private async Task<long> GenerateDoctorCommissionId(string comId)
        {
            string fixedPart = "206";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 DOC_COM_ID FROM MST_DOCTOR_COMMISSION WHERE DOC_COM_ID LIKE @likePattern ORDER BY DOC_COM_ID DESC";

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

                long newDoctorCommissionId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newDoctorCommissionId;
            }
        }


    }
}
