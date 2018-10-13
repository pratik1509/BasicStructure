using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Basic.Services;

namespace Basic.WebAPI.Helpers
{
    public class UserClaims : IUserClaimsService
    {
        private IHttpContextAccessor _contextAccessor;


        public UserClaims(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string Id
        {
            get
            {
                return _contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier.ToString())?.Value;
            }
        }

        public string Email
        {
            get
            {
                return _contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.Email.ToString())?.Value;
            }
        }

        public string Name => _contextAccessor.HttpContext.User.Identity.Name;

        public Dictionary<string, string> GetAllClaims()
        {
            return _contextAccessor.HttpContext.User.Claims
                .ToDictionary(x => x.Type, x => x.Value);
        }
    }
}
