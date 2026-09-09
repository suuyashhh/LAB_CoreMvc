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
    public class HaaSDataCenterRepository : DapperRepositoryBase, IHaaSDataCenter
    {
        private static readonly Random _rng = new Random();

        public HaaSDataCenterRepository(DapperContext context) : base(context) { }

        public async Task<DTODataCenterMetrics> GetLatestMetrics()
        {
            var query = @"
                SELECT TOP 1
                    METRICS_ID,
                    POWER_CONSUMPTION_KW,
                    WASTE_HEAT_GENERATED_KW,
                    TEMPERATURE_CELSIUS,
                    PUE,
                    TIMESTAMP
                FROM [dbo].[HAAS_DATACENTER_METRICS]
                ORDER BY TIMESTAMP DESC";

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<DTODataCenterMetrics>(query);
                return result;
            }
        }

        public async Task<IEnumerable<DTODataCenterMetrics>> GetMetricsHistory(DateTime? fromDate, DateTime? toDate)
        {
            var query = @"
                SELECT
                    METRICS_ID,
                    POWER_CONSUMPTION_KW,
                    WASTE_HEAT_GENERATED_KW,
                    TEMPERATURE_CELSIUS,
                    PUE,
                    TIMESTAMP
                FROM [dbo].[HAAS_DATACENTER_METRICS]
                WHERE (@FromDate IS NULL OR TIMESTAMP >= @FromDate)
                  AND (@ToDate   IS NULL OR TIMESTAMP <= @ToDate)
                ORDER BY TIMESTAMP DESC";

            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<DTODataCenterMetrics>(
                    query, new { FromDate = fromDate, ToDate = toDate });
            }
        }

        public async Task<long> InsertMetrics(DTODataCenterMetrics model)
        {
            var query = @"
                INSERT INTO [dbo].[HAAS_DATACENTER_METRICS]
                    (POWER_CONSUMPTION_KW, WASTE_HEAT_GENERATED_KW, TEMPERATURE_CELSIUS, PUE, TIMESTAMP)
                VALUES
                    (@POWER_CONSUMPTION_KW, @WASTE_HEAT_GENERATED_KW, @TEMPERATURE_CELSIUS, @PUE, @TIMESTAMP);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<long>(query, model);
            }
        }

        /// <summary>
        /// Generates a realistic simulation of data center metrics based on season and outside temp.
        /// A typical large data center runs at 1–5 MW. Waste heat ≈ (PUE - 1) × IT load.
        /// PUE (Power Usage Effectiveness) ranges 1.1–1.6 for modern data centers.
        /// </summary>
        public async Task<DTODataCenterMetrics> SimulateMetrics(double outsideTempCelsius, int season)
        {
            // PUE is higher in summer (less efficient cooling), lower in winter
            double basePue = season == (int)SeasonType.Summer ? 1.45 : 1.2;
            double pue = basePue + (_rng.NextDouble() * 0.1 - 0.05); // ±0.05 noise

            // IT Load: 800–1200 kW (simulating a mid-size data center)
            double powerConsumption = 800 + _rng.NextDouble() * 400;

            // Waste heat = IT load × (PUE - 1)
            double wasteHeat = powerConsumption * (pue - 1.0);

            // Exhaust temperature varies with load and PUE
            double temperature = 25.0 + (pue - 1.0) * 60 + _rng.NextDouble() * 5;

            var metrics = new DTODataCenterMetrics
            {
                POWER_CONSUMPTION_KW       = Math.Round(powerConsumption, 2),
                WASTE_HEAT_GENERATED_KW    = Math.Round(wasteHeat, 2),
                TEMPERATURE_CELSIUS        = Math.Round(temperature, 1),
                PUE                        = Math.Round(pue, 3),
                TIMESTAMP                  = DateTime.UtcNow
            };

            metrics.METRICS_ID = await InsertMetrics(metrics);
            return metrics;
        }
    }
}
