using System.Threading.Tasks;
using Models.Market;

namespace Lab_Mvc.Interfaces.Market
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<bool> ValidateUserAsync(string username, string password);
    }
}
