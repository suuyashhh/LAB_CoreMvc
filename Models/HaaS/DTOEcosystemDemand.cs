using System;

namespace Models.HaaS
{
    /// <summary>
    /// Season types for demand forecasting
    /// </summary>
    public enum SeasonType
    {
        Winter = 1,
        Spring = 2,
        Summer = 3,
        Autumn = 4
    }

    /// <summary>
    /// Ecosystem categories that receive waste heat
    /// </summary>
    public enum EcosystemType
    {
        DistrictHeating = 1,
        Agriculture = 2,
        ThermalStorage = 3,
        Industry = 4
    }

    public class DTOEcosystemDemand
    {
        public long DEMAND_ID { get; set; }
        public string ECOSYSTEM_NAME { get; set; }   // e.g. "DistrictHeating"
        public int ECOSYSTEM_TYPE { get; set; }       // EcosystemType enum value
        public double DEMAND_VALUE_KW { get; set; }
        public double MIN_TEMP_REQUIRED { get; set; } // Minimum heat temp needed (°C)
        public double MAX_TEMP_ACCEPTED { get; set; } // Maximum heat temp accepted (°C)
        public int SEASON_TYPE { get; set; }          // SeasonType enum value
        public string SEASON_NAME { get; set; }       // Readable season label
        public int PRIORITY { get; set; }             // 1=Highest, 4=Lowest
        public DateTime TIMESTAMP { get; set; }
    }
}
