using System;
using System.Collections.Generic;

namespace Models.HaaS
{
    /// <summary>
    /// Result of running the optimization engine.
    /// Shows how total waste heat was allocated across ecosystems.
    /// </summary>
    public class DTOHeatAllocation
    {
        public DateTime TIMESTAMP { get; set; }

        // Supply info
        public double TOTAL_SUPPLY_KW { get; set; }
        public double TOTAL_DEMAND_KW { get; set; }
        public bool IS_SURPLUS { get; set; }           // Supply > Demand
        public double SURPLUS_OR_DEFICIT_KW { get; set; }

        // Per-ecosystem allocation
        public double DISTRICT_HEATING_ALLOCATED_KW { get; set; }
        public double AGRICULTURE_ALLOCATED_KW { get; set; }
        public double THERMAL_STORAGE_ALLOCATED_KW { get; set; }
        public double INDUSTRY_ALLOCATED_KW { get; set; }

        // Efficiency
        public double UTILIZATION_PERCENT { get; set; }

        // Summary log of decisions made
        public List<string> DecisionNotes { get; set; } = new List<string>();

        // Optimization logs generated in this run
        public List<DTOOptimizationLog> Logs { get; set; } = new List<DTOOptimizationLog>();
    }

    /// <summary>
    /// Request model to trigger optimization manually from frontend
    /// </summary>
    public class DTOOptimizationRequest
    {
        public double OverridePowerKW { get; set; }         // 0 = use latest simulated value
        public int SeasonType { get; set; }                 // SeasonType enum value
        public double OutsideTemperatureCelsius { get; set; }
        public bool ForceStorageDischarge { get; set; }     // Override: force storage to discharge
    }
}
