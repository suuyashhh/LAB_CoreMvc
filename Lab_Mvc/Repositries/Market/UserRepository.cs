using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using Dapper;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Market
{
    public class UserRepository : DapperRepositoryBase, IUserRepository
    {
        public UserRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<AppUser?> GetByUsernameAsync(string username)
        {
            var sql = "SELECT id, username, password, role FROM [dbo].[Market_AppUser] WHERE username = @Username";
            return await QueryFirstOrDefaultAsync<AppUser>(sql, new { Username = username });
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null) return false;

            string hashedPassword = HashPassword(password);
            return user.Password == hashedPassword;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
