using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Reflection;

namespace Lab_Mvc.Repositries
{
    public class CasePaperRepository : ICasePaper
    {
        private readonly DapperContext context;

        public CasePaperRepository(DapperContext context)
        {
            //_dbContext = dBContext;
            this.context = context;
        }
        public async Task<IEnumerable<DTOCasePaper>> GetCasePapers()
        {
            try
            {
                var query = QueryConstant.GetCasePapers;

                using (var connection = context.CreateConnection())
                {
                    var CasePapers = await connection.QueryAsync<DTOCasePaper>(query);
                    return CasePapers.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<DTOCasePaper>> GetCasePaperById(long trn_no)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetCasePaperById);
                parameters.Add("@TRN_NO", trn_no);

                using (var connection = context.CreateConnection())
                {
                    var CasePapers = await connection.QueryAsync<DTOCasePaper>(query, parameters);
                    return CasePapers.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveCasePaper(DTOCasePaper casepaper)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertCasePaper);
                parameters.Add("@TRN_NO", casepaper.TRN_NO);
                parameters.Add("@PATIENT_NAME", casepaper.PATIENT_NAME);
                parameters.Add("@GENDER", casepaper.GENDER);
                parameters.Add("@CON_NUMBER", casepaper.CON_NUMBER);
                parameters.Add("@ADDRESS", casepaper.ADDRESS);
                parameters.Add("@DOCTOR_CODE", casepaper.DOCTOR_REF);
                parameters.Add("@DATE", casepaper.DATE);
                parameters.Add("@STATUS_CODE", casepaper.STATUS_CODE);
                parameters.Add("@TOTAL_AMOUNT", casepaper.TOTAL_AMOUNT);
                parameters.Add("@TOTAL_PROFIT", casepaper.TOTAL_PROFIT);
                parameters.Add("@DISCOUNT", casepaper.DISCOUNT);

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

        public async Task EditCasePaper(DTOCasePaper casepaper, long trn_no)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateCasePaper);
                parameters.Add("@TRN_NO", trn_no);
                parameters.Add("@PATIENT_NAME", casepaper.PATIENT_NAME);
                parameters.Add("@GENDER", casepaper.GENDER);
                parameters.Add("@CON_NUMBER", casepaper.CON_NUMBER);
                parameters.Add("@ADDRESS", casepaper.ADDRESS);
                parameters.Add("@DOCTOR_REF", casepaper.DOCTOR_REF);
                parameters.Add("@DATE", casepaper.DATE);
                parameters.Add("@STATUS_CODE", casepaper.STATUS_CODE);
                parameters.Add("@TOTAL_AMOUNT", casepaper.TOTAL_AMOUNT);
                parameters.Add("@TOTAL_PROFIT", casepaper.TOTAL_PROFIT);
                parameters.Add("@DISCOUNT", casepaper.DISCOUNT);



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

        public async Task DeleteCasePaper(long trn_no)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteCasePaper);
                parameters.Add("@TRN_NO", trn_no);



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
