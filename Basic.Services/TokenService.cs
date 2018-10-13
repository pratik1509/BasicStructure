using Mongo.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Basic.Model;

namespace Basic.Services
{
    public class TokenService : BaseMongoRepository, ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDbContext _db;
        private readonly IUserClaimsService _userClaims;

        public TokenService(
            IConfiguration configuration, 
            IMongoDbContext db, 
            IUserClaimsService userClaims) : base(db)
        {
            _configuration = configuration;
            _db = db;
            _userClaims = userClaims;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            var jwtToken = new JwtSecurityToken(
                issuer: _configuration["Token:Issuer"],
                audience: _configuration["Token:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Token:ExpiryInMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:serverSigningPassword"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public bool UpdateRefreshToken(string userId, string oldRefreshToken, string refreshToken)
        {
            var tokensToSave = new List<string>();

            //get existing refresh tokens
            var existingRefreshTokens = ProjectOne<User, List<string>>(x => x.Id == userId, x => x.RefreshTokens);

            if (existingRefreshTokens == null)
            {
                tokensToSave.Add(refreshToken);
            }
            else
            {
                if (!String.IsNullOrEmpty(oldRefreshToken)) //if token already exist, remove it before generating new
                {
                    existingRefreshTokens.Remove(oldRefreshToken);
                }
                else //token not exist, add to existing list
                {
                    existingRefreshTokens.Add(refreshToken);
                }

                tokensToSave = existingRefreshTokens;
            }

            return UpdateOne<User, List<string>>(x => x.Id == userId, x => x.RefreshTokens, tokensToSave, userId);            
        }

        public bool DeleteRefreshToken(string oldRefreshToken, List<string> refreshTokens)
        {          
            if(refreshTokens?.Contains(oldRefreshToken) ?? false)
            {
                refreshTokens.Remove(oldRefreshToken);
                return UpdateOne<User, List<string>>(x => x.Id == _userClaims.Id, x => x.RefreshTokens, refreshTokens, _userClaims.Id);
                //_db.User.UpdateOne(x => x.Id == userId, Builders<User>.Update.Set(x => x.RefreshTokens, refreshTokens));                
            }

            return true;
        }
    }
}