using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.HaaS;
using Models.HaaS;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.HaaS
{
    public class HaaSThermalStorageRepository : DapperRepositoryBase, IHaaSThermalStorage
    {
        public HaaSThermalStorageRepository(DapperContext context) : base(context) { }

        public async Task<DTOThermalStorageStatus> GetCurrentStatus()
        {
            var query = @"
                SELECT TOP 1
                    STORAGE_ID, STORAGE_CAPACITY_KWH, CURRENT_STORED_HEAT_KWH,
                    CHARGE_PERCENTAGE, STATE, STATE_NAME,
                    CHARGING_RATE_KW, DISCHARGING_RATE_KW, TIMESTAMP
                FROM [dbo].[HAAS_THERMAL_STORAGE]
                ORDER BY TIMESTAMP DESC";

            using (var connection = CreateConnection())
                return await connection.QueryFirstOrDefaultAsync<DTOThermalStorageStatus>(query);
        }

        public async Task<IEnumerable<DTOThermalStorageStatus>> GetHistory(int limitRows = 50)
        {
            var query = @"
                SELECT TOP (@Limit)
                    STORAGE_ID, STORAGE_CAPACITY_KWH, CURRENT_STORED_HEAT_KWH,
                    CHARGE_PERCENTAGE, STATE, STATE_NAME,
                    CHARGING_RATE_KW, DISCHARGING_RATE_KW, TIMESTAMP
                FROM [dbo].[HAAS_THERMAL_STORAGE]
                ORDER BY TIMESTAMP DESC";

            using (var connection = CreateConnection())
                return await connection.QueryAsync<DTOThermalStorageStatus>(query, new { Limit = limitRows });
        }

        public async Task<long> UpdateStatus(DTOThermalStorageStatus model)
        {
            model.TIMESTAMP = DateTime.UtcNow;

            // Clamp and compute derived fields
            if (model.STORAGE_CAPACITY_KWH <= 0) model.STORAGE_CAPACITY_KWH = 2000; // Default 2 MWh tank
            model.CURRENT_STORED_HEAT_KWH = Math.Max(0, Math.Min(model.CURRENT_STORED_HEAT_KWH, model.STORAGE_CAPACITY_KWH));
            model.CHARGE_PERCENTAGE = Math.Round((model.CURRENT_STORED_HEAT_KWH / model.STORAGE_CAPACITY_KWH) * 100, 1);

            // Assign state name from enum
            model.STATE_NAME = ((StorageState)model.STATE).ToString();

            var query = @"
                INSERT INTO [dbo].[HAAS_THERMAL_STORAGE]
                    (STORAGE_CAPACITY_KWH, CURRENT_STORED_HEAT_KWH, CHARGE_PERCENTAGE,
                     STATE, STATE_NAME, CHARGING_RATE_KW, DISCHARGING_RATE_KW, TIMESTAMP)
                VALUES
                    (@STORAGE_CAPACITY_KWH, @CURRENT_STORED_HEAT_KWH, @CHARGE_PERCENTAGE,
                     @STATE, @STATE_NAME, @CHARGING_RATE_KW, @DISCHARGING_RATE_KW, @TIMESTAMP);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using (var connection = CreateConnection())
                return await connection.QuerySingleAsync<long>(query, model);
        }
    }
}
