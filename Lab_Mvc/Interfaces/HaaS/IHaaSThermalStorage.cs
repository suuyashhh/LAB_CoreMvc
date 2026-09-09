using Models.HaaS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.HaaS
{
    public interface IHaaSThermalStorage
    {
        Task<DTOThermalStorageStatus> GetCurrentStatus();
        Task<IEnumerable<DTOThermalStorageStatus>> GetHistory(int limitRows = 50);
        Task<long> UpdateStatus(DTOThermalStorageStatus model);
    }
}
