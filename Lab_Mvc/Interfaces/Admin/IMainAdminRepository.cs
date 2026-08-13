using Models.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Admin
{
    public interface IMainAdminRepository
    {
        Task<MainAdmin> LoginMainAdmin(DTOMainAdminLogin loginDto);
        Task<IEnumerable<ModuleUserDto>> GetModuleUsers(string moduleName);
    }
}
