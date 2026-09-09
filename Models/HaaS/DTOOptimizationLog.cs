using System;

namespace Models.HaaS
{
    public class DTOOptimizationLog
    {
        public long LOG_ID { get; set; }
        public DateTime TIMESTAMP { get; set; }
        public string ACTION { get; set; }          // e.g. "REDIRECT_TO_STORAGE", "PRIORITIZE_INDUSTRY"
        public string DETAILS { get; set; }         // Human-readable explanation
        public double HEAT_REDIRECTED_KW { get; set; }
        public string TARGET_ECOSYSTEM { get; set; } // Which ecosystem received heat
        public double SUPPLY_KW { get; set; }        // Total supply at time of decision
        public double TOTAL_DEMAND_KW { get; set; }  // Total demand at time of decision
        public string STATUS { get; set; }           // "SUCCESS" / "PARTIAL" / "DEFICIT"
    }
}
