using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Basic.Services
{
    public interface IPasswordHasher
    {
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        string GenerateIdentityV3Hash(string password, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256, 
        int iterationCount = 10000, int saltSize = 16);
        bool VerifyIdentityV3Hash(string password, string passwordHash);
    }
}