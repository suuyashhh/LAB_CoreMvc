using System;

namespace Models.HaaS
{
    /// <summary>
    /// Current state of the thermal storage tank
    /// </summary>
    public enum StorageState
    {
        Idle = 0,
        Charging = 1,
        Discharging = 2
    }

    public class DTOThermalStorageStatus
    {
        public long STORAGE_ID { get; set; }
        public double STORAGE_CAPACITY_KWH { get; set; }     // Max capacity
        public double CURRENT_STORED_HEAT_KWH { get; set; }  // Current stored amount
        public double CHARGE_PERCENTAGE { get; set; }         // 0–100%
        public int STATE { get; set; }                        // StorageState enum
        public string STATE_NAME { get; set; }                // "Charging" / "Discharging" / "Idle"
        public double CHARGING_RATE_KW { get; set; }          // kW rate if charging
        public double DISCHARGING_RATE_KW { get; set; }       // kW rate if discharging
        public DateTime TIMESTAMP { get; set; }
    }
}
