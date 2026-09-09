using Models.HaaS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.HaaS
{
    public interface IHaaSDataCenter
    {
        Task<DTODataCenterMetrics> GetLatestMetrics();
        Task<IEnumerable<DTODataCenterMetrics>> GetMetricsHistory(DateTime? fromDate, DateTime? toDate);
        Task<long> InsertMetrics(DTODataCenterMetrics model);
        Task<DTODataCenterMetrics> SimulateMetrics(double outsideTempCelsius, int season);
    }
}
