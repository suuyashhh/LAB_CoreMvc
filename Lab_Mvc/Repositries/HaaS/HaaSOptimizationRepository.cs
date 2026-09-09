using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.HaaS;
using Models.HaaS;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.HaaS
{
    /// <summary>
    /// Core Optimization Engine for Heat as a Service (HaaS).
    ///
    /// Rules:
    ///   1. Priority order: Industry (1) → District Heating (2) → Agriculture (3) → Thermal Storage (4)
    ///   2. If supply > total demand → route surplus to Thermal Storage (charge tank)
    ///   3. If supply < total demand → allocate by priority, cutting lower-priority ecosystems first
    ///   4. Temperature constraints are enforced (min/max °C per ecosystem)
    ///   5. All decisions are logged to HAAS_OPTIMIZATION_LOG
    /// </summary>
    public class HaaSOptimizationRepository : DapperRepositoryBase, IHaaSOptimization
    {
        private readonly IHaaSDataCenter _dataCenter;
        private readonly IHaaSEcosystem _ecosystem;
        private readonly IHaaSThermalStorage _storage;

        public HaaSOptimizationRepository(
            DapperContext context,
            IHaaSDataCenter dataCenter,
            IHaaSEcosystem ecosystem,
            IHaaSThermalStorage storage)
            : base(context)
        {
            _dataCenter = dataCenter;
            _ecosystem  = ecosystem;
            _storage    = storage;
        }

        public async Task<IEnumerable<DTOOptimizationLog>> GetOptimizationLogs(DateTime? fromDate, DateTime? toDate, int limitRows = 100)
        {
            var query = @"
                SELECT TOP (@Limit)
                    LOG_ID, TIMESTAMP, ACTION, DETAILS, HEAT_REDIRECTED_KW,
                    TARGET_ECOSYSTEM, SUPPLY_KW, TOTAL_DEMAND_KW, STATUS
                FROM [dbo].[HAAS_OPTIMIZATION_LOG]
                WHERE (@FromDate IS NULL OR TIMESTAMP >= @FromDate)
                  AND (@ToDate   IS NULL OR TIMESTAMP <= @ToDate)
                ORDER BY TIMESTAMP DESC";

            using (var connection = CreateConnection())
                return await connection.QueryAsync<DTOOptimizationLog>(
                    query, new { FromDate = fromDate, ToDate = toDate, Limit = limitRows });
        }

        public async Task<long> InsertLog(DTOOptimizationLog log)
        {
            log.TIMESTAMP = DateTime.UtcNow;

            var query = @"
                INSERT INTO [dbo].[HAAS_OPTIMIZATION_LOG]
                    (TIMESTAMP, ACTION, DETAILS, HEAT_REDIRECTED_KW, TARGET_ECOSYSTEM,
                     SUPPLY_KW, TOTAL_DEMAND_KW, STATUS)
                VALUES
                    (@TIMESTAMP, @ACTION, @DETAILS, @HEAT_REDIRECTED_KW, @TARGET_ECOSYSTEM,
                     @SUPPLY_KW, @TOTAL_DEMAND_KW, @STATUS);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using (var connection = CreateConnection())
                return await connection.QuerySingleAsync<long>(query, log);
        }

        /// <summary>
        /// Core optimization algorithm.
        /// </summary>
        public async Task<DTOHeatAllocation> RunOptimization(DTOOptimizationRequest request)
        {
            // ── Step 1: Get / Simulate metrics ────────────────────────────────────────
            DTODataCenterMetrics metrics;
            if (request.OverridePowerKW > 0)
            {
                // Use override value; waste heat ≈ (PUE-1) * powerKW, PUE ~1.3
                metrics = new DTODataCenterMetrics
                {
                    POWER_CONSUMPTION_KW    = request.OverridePowerKW,
                    WASTE_HEAT_GENERATED_KW = Math.Round(request.OverridePowerKW * 0.3, 2),
                    TEMPERATURE_CELSIUS     = 40,
                    PUE                     = 1.3,
                    TIMESTAMP               = DateTime.UtcNow
                };
            }
            else
            {
                metrics = await _dataCenter.SimulateMetrics(
                    request.OutsideTemperatureCelsius, request.SeasonType);
            }

            double supplyKW = metrics.WASTE_HEAT_GENERATED_KW;
            double heatTempC = metrics.TEMPERATURE_CELSIUS;

            // ── Step 2: Fetch ecosystem demands for the selected season ───────────────
            var demands = (await _ecosystem.GetDemandsBySeason(request.SeasonType))
                          .OrderBy(d => d.PRIORITY)
                          .ToList();

            if (!demands.Any())
            {
                // Auto-seed defaults on first run
                await _ecosystem.SeedDefaultDemands();
                demands = (await _ecosystem.GetDemandsBySeason(request.SeasonType))
                          .OrderBy(d => d.PRIORITY)
                          .ToList();
            }

            double totalDemandKW = demands.Sum(d => d.DEMAND_VALUE_KW);
            double remainingSupply = supplyKW;

            var allocation = new DTOHeatAllocation
            {
                TIMESTAMP       = DateTime.UtcNow,
                TOTAL_SUPPLY_KW = supplyKW,
                TOTAL_DEMAND_KW = totalDemandKW
            };

            // ── Step 3: Allocate by priority ──────────────────────────────────────────
            var logTasks = new List<Task>();

            foreach (var demand in demands)
            {
                if (remainingSupply <= 0) break;

                // Check temperature compatibility
                if (heatTempC < demand.MIN_TEMP_REQUIRED || heatTempC > demand.MAX_TEMP_ACCEPTED)
                {
                    allocation.DecisionNotes.Add(
                        $"⚠ Skipped {demand.ECOSYSTEM_NAME}: heat temp {heatTempC:F1}°C outside range [{demand.MIN_TEMP_REQUIRED}–{demand.MAX_TEMP_ACCEPTED}°C]");
                    continue;
                }

                double allocated = Math.Min(demand.DEMAND_VALUE_KW, remainingSupply);
                remainingSupply -= allocated;

                // Assign to the right field
                switch ((EcosystemType)demand.ECOSYSTEM_TYPE)
                {
                    case EcosystemType.DistrictHeating: allocation.DISTRICT_HEATING_ALLOCATED_KW = allocated; break;
                    case EcosystemType.Agriculture:     allocation.AGRICULTURE_ALLOCATED_KW      = allocated; break;
                    case EcosystemType.ThermalStorage:  allocation.THERMAL_STORAGE_ALLOCATED_KW  = allocated; break;
                    case EcosystemType.Industry:        allocation.INDUSTRY_ALLOCATED_KW          = allocated; break;
                }

                string status = allocated >= demand.DEMAND_VALUE_KW ? "SUCCESS" : "PARTIAL";
                allocation.DecisionNotes.Add(
                    $"✓ {demand.ECOSYSTEM_NAME}: allocated {allocated:F1} kW / requested {demand.DEMAND_VALUE_KW:F1} kW [{status}]");

                var log = new DTOOptimizationLog
                {
                    ACTION              = $"ALLOCATE_TO_{demand.ECOSYSTEM_NAME.ToUpper()}",
                    DETAILS             = $"Season: {demand.SEASON_NAME}. Allocated {allocated:F1} kW to {demand.ECOSYSTEM_NAME} (demand was {demand.DEMAND_VALUE_KW:F1} kW).",
                    HEAT_REDIRECTED_KW  = allocated,
                    TARGET_ECOSYSTEM    = demand.ECOSYSTEM_NAME,
                    SUPPLY_KW           = supplyKW,
                    TOTAL_DEMAND_KW     = totalDemandKW,
                    STATUS              = status
                };
                logTasks.Add(Task.Run(async () => {
                    log.LOG_ID = await InsertLog(log);
                    allocation.Logs.Add(log);
                }));
            }

            // ── Step 4: Handle surplus → charge thermal storage ───────────────────────
            if (remainingSupply > 0.5)
            {
                allocation.THERMAL_STORAGE_ALLOCATED_KW += remainingSupply;
                allocation.DecisionNotes.Add(
                    $"⚡ Surplus {remainingSupply:F1} kW redirected to Thermal Storage (tank charging)");

                // Update storage state
                var currentStorage = await _storage.GetCurrentStatus();
                double capacityKWh = currentStorage?.STORAGE_CAPACITY_KWH ?? 2000;
                double storedKWh   = currentStorage?.CURRENT_STORED_HEAT_KWH ?? 0;

                // Add surplus (assume 1-hour simulation window)
                storedKWh = Math.Min(storedKWh + remainingSupply, capacityKWh);

                var log = new DTOOptimizationLog
                {
                    ACTION             = "REDIRECT_TO_STORAGE",
                    DETAILS            = $"Surplus {remainingSupply:F1} kW routed to thermal storage tank.",
                    HEAT_REDIRECTED_KW = remainingSupply,
                    TARGET_ECOSYSTEM   = "ThermalStorage",
                    SUPPLY_KW          = supplyKW,
                    TOTAL_DEMAND_KW    = totalDemandKW,
                    STATUS             = "SUCCESS"
                };
                log.LOG_ID = await InsertLog(log);
                allocation.Logs.Add(log);

                await _storage.UpdateStatus(new DTOThermalStorageStatus
                {
                    STORAGE_CAPACITY_KWH    = capacityKWh,
                    CURRENT_STORED_HEAT_KWH = storedKWh,
                    STATE                   = (int)StorageState.Charging,
                    CHARGING_RATE_KW        = remainingSupply
                });
            }
            else if (request.ForceStorageDischarge)
            {
                // Discharge storage to cover deficit
                var currentStorage = await _storage.GetCurrentStatus();
                if (currentStorage != null && currentStorage.CURRENT_STORED_HEAT_KWH > 0)
                {
                    double discharge = Math.Min(currentStorage.CURRENT_STORED_HEAT_KWH, Math.Abs(remainingSupply));
                    allocation.DecisionNotes.Add($"🔋 Storage discharging {discharge:F1} kWh to cover demand deficit.");

                    await _storage.UpdateStatus(new DTOThermalStorageStatus
                    {
                        STORAGE_CAPACITY_KWH    = currentStorage.STORAGE_CAPACITY_KWH,
                        CURRENT_STORED_HEAT_KWH = currentStorage.CURRENT_STORED_HEAT_KWH - discharge,
                        STATE                   = (int)StorageState.Discharging,
                        DISCHARGING_RATE_KW     = discharge
                    });
                }
            }
            else
            {
                // Idle storage
                var currentStorage = await _storage.GetCurrentStatus();
                if (currentStorage != null)
                {
                    await _storage.UpdateStatus(new DTOThermalStorageStatus
                    {
                        STORAGE_CAPACITY_KWH    = currentStorage.STORAGE_CAPACITY_KWH,
                        CURRENT_STORED_HEAT_KWH = currentStorage.CURRENT_STORED_HEAT_KWH,
                        STATE                   = (int)StorageState.Idle
                    });
                }
            }

            await Task.WhenAll(logTasks);

            // ── Step 5: Final metrics ─────────────────────────────────────────────────
            double totalAllocated = allocation.DISTRICT_HEATING_ALLOCATED_KW
                                  + allocation.AGRICULTURE_ALLOCATED_KW
                                  + allocation.THERMAL_STORAGE_ALLOCATED_KW
                                  + allocation.INDUSTRY_ALLOCATED_KW;

            allocation.IS_SURPLUS             = supplyKW >= totalDemandKW;
            allocation.SURPLUS_OR_DEFICIT_KW  = Math.Round(supplyKW - totalDemandKW, 2);
            allocation.UTILIZATION_PERCENT    = supplyKW > 0
                                               ? Math.Round((totalAllocated / supplyKW) * 100, 1)
                                               : 0;

            return allocation;
        }
    }
}
