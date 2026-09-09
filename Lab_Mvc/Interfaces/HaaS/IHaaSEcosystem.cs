using Models.HaaS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.HaaS
{
    public interface IHaaSEcosystem
    {
        Task<IEnumerable<DTOEcosystemDemand>> GetAllDemands();
        Task<IEnumerable<DTOEcosystemDemand>> GetDemandsBySeason(int seasonType);
        Task<DTOEcosystemDemand> GetDemandByEcosystemAndSeason(int ecosystemType, int seasonType);
        Task<long> UpsertDemand(DTOEcosystemDemand model);
        Task SeedDefaultDemands();
    }
}
