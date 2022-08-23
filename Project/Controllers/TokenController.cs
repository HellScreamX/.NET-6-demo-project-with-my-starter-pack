using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ndif.BusinessLogicLayer.CustomModels;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ProjectUser> _userManager;
        private readonly SignInManager<ProjectUser> _signInManager;
        private readonly ITokenManager _ITokenManager;


        public TokenController(UserManager<ProjectUser> userManager, SignInManager<ProjectUser> signInManager, 
            ITokenManager iTokenManager)
        {
            _ITokenManager = iTokenManager;
            _userManager = userManager;
            _signInManager = signInManager;
           
        }

        [AllowAnonymous]
        [HttpPost("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody]LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(login.Username);
                if (user != null)
                {
                    //check if user Email is confirmed
                    if(!user.EmailConfirmed) throw new CustomException(ErrorCode.UnconfirmedEmail);

                    var token =  await _ITokenManager.BuildFullTokenAsync(user);

                    return Ok(token);
                }
                else
                {
                    return BadRequest("user not found");
                }
            }
            else
            {
                throw new CustomException(ErrorCode.InvalidCredentials);
            }

        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody]TokensInfos tokensInfos)
        {
            var principal = _ITokenManager.GetPrincipalFromExpiredToken(tokensInfos.accessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var savedRefreshToken = await _ITokenManager.GetRefreshTokenAsync(userId); //retrieve the refresh token from a data store
            if (savedRefreshToken != tokensInfos.refreshToken)
                throw new SecurityTokenException("Invalid refresh token");
            
            //=====================================================New Access Token=============
            var user = await _userManager.FindByIdAsync(userId);
            var claimPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = claimPrincipal.Identity as ClaimsIdentity??throw new CustomException("no identity was fine for the token");
            var newJwtToken = _ITokenManager.GenerateAccessToken(identity.Claims);

            //======================New refresh Token========================================
            var newRefreshToken = _ITokenManager.GenerateRefreshToken();
            await _ITokenManager.ReplaceRefreshTokenAsync(userId, newRefreshToken);

            return new ObjectResult(new
            {
                accessToken = newJwtToken,
                refreshToken = newRefreshToken
            });
        }


        [HttpPost("ShortToken")]
        public async Task<IActionResult> ShortToken([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(login.Username);

                if (user != null)
                {
                    //check if user Email is confirmed
                    if (!user.EmailConfirmed) throw new CustomException(ErrorCode.UnconfirmedEmail);

                    //======================New Access Token=============

                    var claimPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                    var identity = (ClaimsIdentity)claimPrincipal.Identity!;
                    var newJwtToken = _ITokenManager.GenerateAccessToken(identity.Claims, 5);

                    //======================New refresh Token========================================
                    var newRefreshToken = _ITokenManager.GenerateRefreshToken();
                    await _ITokenManager.ReplaceRefreshTokenAsync(user.Id, newRefreshToken);

                    return new ObjectResult(new
                    {
                        accessToken = newJwtToken,
                        refreshToken = newRefreshToken
                    });
                }
                else
                {
                    return BadRequest("user not found");
                }
            }
            else
            {
                throw new CustomException(ErrorCode.InvalidCredentials);
            }
            
        }

    }
}