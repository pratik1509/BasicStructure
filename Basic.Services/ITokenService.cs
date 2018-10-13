using System.Collections.Generic;
using System.Security.Claims;

namespace Basic.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool UpdateRefreshToken(string userId, string oldRefreshToken, string refreshToken);
        bool DeleteRefreshToken(string oldRefreshToken, List<string> refreshToken);
    }   
}