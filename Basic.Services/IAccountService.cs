using System.Collections.Generic;
using System.Threading.Tasks;
using Basic.Model;

namespace Basic.Services
{
    public interface IAccountService
    {
        Task<bool> IsExist(string email);
        Task<User> GetUser(string email);
        Task<string> AddUser(User user);
        Task<List<string>> GetRefreshTokens();
    }
}