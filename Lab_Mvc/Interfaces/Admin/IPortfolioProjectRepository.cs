using Models.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Admin
{
    public interface IPortfolioProjectRepository
    {
        Task<IEnumerable<PortfolioProject>> GetAllProjectsAsync(bool onlyActive = false);
        Task<PortfolioProject> GetProjectByIdAsync(int id);
        Task<int> AddProjectAsync(PortfolioProject project);
        Task<int> UpdateProjectAsync(PortfolioProject project);
        Task<int> DeleteProjectAsync(int id);
    }
}
