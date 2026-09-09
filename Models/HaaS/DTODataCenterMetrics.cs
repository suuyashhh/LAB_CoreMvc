using System;

namespace Models.HaaS
{
    public class DTODataCenterMetrics
    {
        public long METRICS_ID { get; set; }
        public double POWER_CONSUMPTION_KW { get; set; }
        public double WASTE_HEAT_GENERATED_KW { get; set; }
        public double TEMPERATURE_CELSIUS { get; set; }
        public double PUE { get; set; }  // Power Usage Effectiveness
        public DateTime TIMESTAMP { get; set; }
    }
}
