using Models.HaaS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.HaaS
{
    public interface IHaaSOptimization
    {
        Task<DTOHeatAllocation> RunOptimization(DTOOptimizationRequest request);
        Task<IEnumerable<DTOOptimizationLog>> GetOptimizationLogs(DateTime? fromDate, DateTime? toDate, int limitRows = 100);
        Task<long> InsertLog(DTOOptimizationLog log);
    }
}
