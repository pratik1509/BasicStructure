using Mongo.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Basic.Model;

namespace Basic.Services
{
    public class AccountService : BaseMongoRepository, IAccountService
    {
        private readonly IMongoDbContext _db;
        private readonly IUserClaimsService _userClaims;

        public AccountService(IMongoDbContext db, IUserClaimsService userClaims) : base(db)
        {
            _db = db;
            _userClaims = userClaims;
        }

        public async Task<bool> IsExist(string email)
        {
            return await AnyAsync<User>(x => x.Email == email);
        }

        public async Task<User> GetUser(string email)
        {
            return await GetOneAsync<User>(x => x.Email == email);
        }

        public async Task<List<string>> GetRefreshTokens()
        {
            return await ProjectOneAsync<User, List<string>>
                (x => x.Email == _userClaims.Email, x => x.RefreshTokens);
        }

        public async Task<string> AddUser(User user)
        {
            return await AddOneAsync(user, string.Empty);
        }
    }
}