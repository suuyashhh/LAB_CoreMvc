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
    public class HaaSEcosystemRepository : DapperRepositoryBase, IHaaSEcosystem
    {
        public HaaSEcosystemRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<DTOEcosystemDemand>> GetAllDemands()
        {
            var query = @"
                SELECT
                    DEMAND_ID, ECOSYSTEM_NAME, ECOSYSTEM_TYPE,
                    DEMAND_VALUE_KW, MIN_TEMP_REQUIRED, MAX_TEMP_ACCEPTED,
                    SEASON_TYPE, SEASON_NAME, PRIORITY, TIMESTAMP
                FROM [dbo].[HAAS_ECOSYSTEM_DEMAND]
                ORDER BY SEASON_TYPE, PRIORITY";

            using (var connection = CreateConnection())
                return await connection.QueryAsync<DTOEcosystemDemand>(query);
        }

        public async Task<IEnumerable<DTOEcosystemDemand>> GetDemandsBySeason(int seasonType)
        {
            var query = @"
                SELECT
                    DEMAND_ID, ECOSYSTEM_NAME, ECOSYSTEM_TYPE,
                    DEMAND_VALUE_KW, MIN_TEMP_REQUIRED, MAX_TEMP_ACCEPTED,
                    SEASON_TYPE, SEASON_NAME, PRIORITY, TIMESTAMP
                FROM [dbo].[HAAS_ECOSYSTEM_DEMAND]
                WHERE SEASON_TYPE = @SeasonType
                ORDER BY PRIORITY";

            using (var connection = CreateConnection())
                return await connection.QueryAsync<DTOEcosystemDemand>(query, new { SeasonType = seasonType });
        }

        public async Task<DTOEcosystemDemand> GetDemandByEcosystemAndSeason(int ecosystemType, int seasonType)
        {
            var query = @"
                SELECT TOP 1
                    DEMAND_ID, ECOSYSTEM_NAME, ECOSYSTEM_TYPE,
                    DEMAND_VALUE_KW, MIN_TEMP_REQUIRED, MAX_TEMP_ACCEPTED,
                    SEASON_TYPE, SEASON_NAME, PRIORITY, TIMESTAMP
                FROM [dbo].[HAAS_ECOSYSTEM_DEMAND]
                WHERE ECOSYSTEM_TYPE = @EcosystemType AND SEASON_TYPE = @SeasonType";

            using (var connection = CreateConnection())
                return await connection.QueryFirstOrDefaultAsync<DTOEcosystemDemand>(
                    query, new { EcosystemType = ecosystemType, SeasonType = seasonType });
        }

        public async Task<long> UpsertDemand(DTOEcosystemDemand model)
        {
            model.TIMESTAMP = DateTime.UtcNow;

            var query = @"
                IF EXISTS (SELECT 1 FROM [dbo].[HAAS_ECOSYSTEM_DEMAND]
                           WHERE ECOSYSTEM_TYPE = @ECOSYSTEM_TYPE AND SEASON_TYPE = @SEASON_TYPE)
                BEGIN
                    UPDATE [dbo].[HAAS_ECOSYSTEM_DEMAND]
                    SET DEMAND_VALUE_KW    = @DEMAND_VALUE_KW,
                        MIN_TEMP_REQUIRED  = @MIN_TEMP_REQUIRED,
                        MAX_TEMP_ACCEPTED  = @MAX_TEMP_ACCEPTED,
                        PRIORITY           = @PRIORITY,
                        TIMESTAMP          = @TIMESTAMP
                    WHERE ECOSYSTEM_TYPE = @ECOSYSTEM_TYPE AND SEASON_TYPE = @SEASON_TYPE;
                    SELECT DEMAND_ID FROM [dbo].[HAAS_ECOSYSTEM_DEMAND]
                    WHERE ECOSYSTEM_TYPE = @ECOSYSTEM_TYPE AND SEASON_TYPE = @SEASON_TYPE;
                END
                ELSE
                BEGIN
                    INSERT INTO [dbo].[HAAS_ECOSYSTEM_DEMAND]
                        (ECOSYSTEM_NAME, ECOSYSTEM_TYPE, DEMAND_VALUE_KW, MIN_TEMP_REQUIRED,
                         MAX_TEMP_ACCEPTED, SEASON_TYPE, SEASON_NAME, PRIORITY, TIMESTAMP)
                    VALUES
                        (@ECOSYSTEM_NAME, @ECOSYSTEM_TYPE, @DEMAND_VALUE_KW, @MIN_TEMP_REQUIRED,
                         @MAX_TEMP_ACCEPTED, @SEASON_TYPE, @SEASON_NAME, @PRIORITY, @TIMESTAMP);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
                END";

            using (var connection = CreateConnection())
                return await connection.QuerySingleAsync<long>(query, model);
        }

        /// <summary>
        /// Inserts realistic default demand values for all 4 ecosystems × 4 seasons.
        /// Designed to reflect real-world heat demand patterns.
        /// </summary>
        public async Task SeedDefaultDemands()
        {
            // (EcosystemType, SeasonType, DemandKW, MinTemp, MaxTemp, Priority, EcosystemName, SeasonName)
            var seeds = new (int eco, int season, double demandKW, double minT, double maxT, int priority, string ecoName, string seasonName)[]
            {
                // District Heating: HIGH in winter, very LOW in summer
                (1, 1,  450, 30, 90,  2, "DistrictHeating", "Winter"),
                (1, 2,  200, 30, 90,  2, "DistrictHeating", "Spring"),
                (1, 3,   50, 30, 90,  2, "DistrictHeating", "Summer"),
                (1, 4,  300, 30, 90,  2, "DistrictHeating", "Autumn"),

                // Agriculture / Greenhouses: moderate year-round, higher in winter (cold outside)
                (2, 1,  180, 20, 45,  3, "Agriculture",     "Winter"),
                (2, 2,  120, 20, 45,  3, "Agriculture",     "Spring"),
                (2, 3,   80, 20, 45,  3, "Agriculture",     "Summer"),
                (2, 4,  150, 20, 45,  3, "Agriculture",     "Autumn"),

                // Thermal Storage: absorbs excess heat; demand varies by surplus
                (3, 1,  100, 25, 95,  4, "ThermalStorage",  "Winter"),
                (3, 2,  200, 25, 95,  4, "ThermalStorage",  "Spring"),
                (3, 3,  300, 25, 95,  4, "ThermalStorage",  "Summer"),
                (3, 4,  150, 25, 95,  4, "ThermalStorage",  "Autumn"),

                // Industry / Foundries: constant high-temp demand year-round
                (4, 1,  250, 35, 120, 1, "Industry",        "Winter"),
                (4, 2,  250, 35, 120, 1, "Industry",        "Spring"),
                (4, 3,  250, 35, 120, 1, "Industry",        "Summer"),
                (4, 4,  250, 35, 120, 1, "Industry",        "Autumn"),
            };

            foreach (var s in seeds)
            {
                await UpsertDemand(new DTOEcosystemDemand
                {
                    ECOSYSTEM_TYPE    = s.eco,
                    ECOSYSTEM_NAME    = s.ecoName,
                    SEASON_TYPE       = s.season,
                    SEASON_NAME       = s.seasonName,
                    DEMAND_VALUE_KW   = s.demandKW,
                    MIN_TEMP_REQUIRED = s.minT,
                    MAX_TEMP_ACCEPTED = s.maxT,
                    PRIORITY          = s.priority
                });
            }
        }
    }
}
