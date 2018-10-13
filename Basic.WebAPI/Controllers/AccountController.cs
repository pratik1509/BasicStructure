using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Basic.Model;
using Basic.Services;
using Basic.WebAPI.Filters;
using Basic.WebAPI.Helpers;
using Basic.WebAPI.ViewModels;

namespace Basic.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        
        public AccountController(
        IMapper mapper,
        IAccountService authService,
        IPasswordHasher passworHasher,
        ITokenService tokenService)
        {            
            _accountService = authService;
            _passwordHasher = passworHasher;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody]LoginVm loginVm)
        {
            var res = await _accountService.GetUser(loginVm.Email);

            if (res?.PasswordHash == null || !_passwordHasher.VerifyIdentityV3Hash(loginVm.Password, res.PasswordHash))
            {
                return new BadRequestObjectResult(AccountError.InvalidEmailOrPassword);
            }

            //generate token and refresh token and return to client
            return GetToken(res.FirstName, res.LastName, res.Id, res.Email, res.Type);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Signup([FromBody]SignupVm signupVm)
        {
            //check if user already exists
            var existingUser = await _accountService.GetUser(signupVm.Email);

            if (existingUser != null)
            {
                return new BadRequestObjectResult(AccountError.UserAlreadyExist);
            }

            //create passwordhash for storing in db
            var passwordHash = _passwordHasher.GenerateIdentityV3Hash(signupVm.Password);

            var user = _mapper.Map<User>(signupVm);
            user.PasswordHash = passwordHash;
            user.Type = UserType.Doctor; // assuming only doctor will use this service to register

            var newUserId = await _accountService.AddUser(user);

            return Ok(newUserId);
        }

        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> Logout(LogoutVm logoutVm)
        {
            //if user exists fetch all existing refresh tokens
            var existingRefreshTokens = await _accountService.GetRefreshTokens();

            if (existingRefreshTokens != null)
            {
                //remove provided refresh token
                var result = _tokenService
                    .DeleteRefreshToken(logoutVm.RefreshTokenToDelete,
                    existingRefreshTokens);
            }

            return Ok();
        }

        #region Private Methods

        private IActionResult GetToken(string firstName, string lastName, string userId, string email, UserType type)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, firstName + " " + lastName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, type.ToString())
            };

            //generating token and refresh token
            var jwtToken = _tokenService.GenerateAccessToken(userClaims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _tokenService.UpdateRefreshToken(userId, string.Empty, refreshToken);

            var generatedTokens = new ObjectResult(new
            {
                token = jwtToken,
                refreshToken = refreshToken
            });

            return Ok(generatedTokens);
        }

        #endregion
    }
}
