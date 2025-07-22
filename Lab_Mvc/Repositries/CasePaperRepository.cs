using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;   
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Reflection;
using System.Globalization;

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
        public async Task<IEnumerable<DTOCasePaper>> GetCasePapers(int comId)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetCasePapers);
                parameters.Add("@COM_ID", comId);

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

        public async Task<DTOCasePaper> GetCasePaperById(Int64 trn_no)
        {
            try
            {
                var query = "sp_master";
                using (var connection = context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", QueryConstant.GetCasePaperById);
                    parameters.Add("@TRN_NO", trn_no);

                    using (var multi = await connection.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure))
                    {
                        var casepaper = await multi.ReadFirstOrDefaultAsync<DTOCasePaper>();
                        if (casepaper != null)
                        {
                            var testItems = (await multi.ReadAsync<DTOTestTable>()).ToList();
                            casepaper.MatIs = testItems;
                        }

                        return casepaper;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SaveCasePaper(DTOCasePaper casepaper)
        {
            try
            {
                var query = "sp_master";
                using (var connection = context.CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            Int64 newPatientId = await GenerateNewPatientId(casepaper.COM_ID);

                            // Step 1: Save CasePaper
                            var parameters = new DynamicParameters();
                            parameters.Add("@Action", QueryConstant.InsertCasePaper);
                            parameters.Add("@TRN_NO", newPatientId);
                            parameters.Add("@PATIENT_NAME", casepaper.PATIENT_NAME);
                            parameters.Add("@GENDER", casepaper.GENDER);
                            parameters.Add("@CON_NUMBER", casepaper.CON_NUMBER);
                            parameters.Add("@ADDRESS", casepaper.ADDRESS);
                            parameters.Add("@DOCTOR_CODE", casepaper.DOCTOR_CODE);
                            parameters.Add("@DATE", casepaper.DATE);
                            parameters.Add("@STATUS_CODE", casepaper.STATUS_CODE);
                            parameters.Add("@TOTAL_AMOUNT", casepaper.TOTAL_AMOUNT);
                            parameters.Add("@TOTAL_PROFIT", casepaper.TOTAL_PROFIT);
                            parameters.Add("@DISCOUNT", casepaper.DISCOUNT);
                            parameters.Add("@COM_ID", casepaper.COM_ID);
                            parameters.Add("@PAYMENT_AMOUNT", casepaper.PAYMENT_AMOUNT);
                            parameters.Add("@PAYMENT_METHOD", casepaper.PAYMENT_METHOD);
                            parameters.Add("@COLLECTION_TYPE", casepaper.COLLECTION_TYPE);
                            parameters.Add("@PAYMENT_STATUS", casepaper.PAYMENT_STATUS);

                            await connection.ExecuteAsync(query, parameters, transaction, commandType: CommandType.StoredProcedure);

                            // Step 2: Prepare TestTableType DataTable
                            if (casepaper.MatIs != null && casepaper.MatIs.Any())
                            {
                                var testTable = new DataTable();
                                testTable.Columns.Add("TEST_CODE", typeof(Int64));
                                testTable.Columns.Add("TRN_NO", typeof(Int64));
                                testTable.Columns.Add("SR_NO", typeof(int));
                                testTable.Columns.Add("PRICE", typeof(decimal));
                                testTable.Columns.Add("LAB_PRICE", typeof(decimal));
                                testTable.Columns.Add("COM_ID", typeof(int));

                                int srNo = 1;
                                foreach (var test in casepaper.MatIs)
                                {
                                    testTable.Rows.Add(
                                        test.TEST_CODE,
                                        newPatientId,
                                        srNo++,
                                        test.PRICE,
                                        test.LAB_PRICE,
                                        casepaper.COM_ID
                                    );
                                }

                                var testParams = new DynamicParameters();
                                testParams.Add("@Action", "InsertCasePaperTests");
                                testParams.Add("@TestItems", testTable.AsTableValuedParameter("dbo.TestTableType"));

                                await connection.ExecuteAsync(query, testParams, transaction, commandType: CommandType.StoredProcedure);
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
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
                parameters.Add("@DOCTOR_REF", casepaper.DOCTOR_CODE);
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


        private async Task<long> GenerateNewPatientId(string comId)
        {
            // Format: yyMMdd (e.g., 250507 for May 7, 2025)
            string datePart = DateTime.UtcNow.AddHours(5.5).ToString("yyMMdd", CultureInfo.InvariantCulture);
            string dateComboKey = datePart + comId;

            string query = "SELECT TOP 1 TRN_NO FROM MST_PATIENT WHERE TRN_NO LIKE @key + '%' ORDER BY TRN_NO DESC";

            using (var conn = context.CreateConnection())
            {
                string lastId = await conn.ExecuteScalarAsync<string>(query, new { key = dateComboKey });

                int nextNum = 1;
                if (!string.IsNullOrEmpty(lastId))
                {
                    // Extract the numeric suffix after date+comId (assumes 9-character prefix)
                    if (int.TryParse(lastId.Substring(9), out int lastNum))
                        nextNum = lastNum + 1;
                }

                // Final TRN_NO: date + comId + nextNum (e.g., 250507101)
                return long.Parse(dateComboKey + nextNum);
            }
        }



    }
}
