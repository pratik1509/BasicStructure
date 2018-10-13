using System.Collections.Generic;

namespace Basic.Services
{
    public interface IUserClaimsService
    {
        string Id { get; }
        string Name { get; }
        string Email { get; }

        Dictionary<string, string> GetAllClaims();
    }
}