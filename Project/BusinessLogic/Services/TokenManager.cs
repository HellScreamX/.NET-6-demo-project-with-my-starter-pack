
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Ndif.BusinessLogicLayer.CustomModels;
using Project.DAL;

public interface ITokenManager
{
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GenerateAccessToken(IEnumerable<Claim> claims,int minutes = 60);
    string GenerateRefreshToken();
    Task<string> GetRefreshTokenAsync(string userId);
    Task ReplaceRefreshTokenAsync(string userId, string refreshToken);
    Task<FullToken> BuildFullTokenAsync(ProjectUser user);
}
public class TokenManager : ITokenManager
{
    private IConfiguration _config;
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ProjectUser> _signInManager;

    public TokenManager(ApplicationDbContext context, IConfiguration config, SignInManager<ProjectUser> signInManager)
    {
        _config = config;
        _context=context;
        _signInManager = signInManager;
    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
    public string GenerateAccessToken(IEnumerable<Claim> claims, int minutes = 120)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],

          _config["Jwt:Issuer"],
          claims,
          expires: DateTime.Now.AddMinutes(minutes),
          signingCredentials: creds
          );
        return new JwtSecurityTokenHandler().WriteToken(token);
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

    public async Task<string> GetRefreshTokenAsync(string userId)
    {
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new CustomException(ErrorCode.UserNotfound);
        return user.RefreshToken?? throw new CustomException("refresh token is null");
    }

    public async Task ReplaceRefreshTokenAsync(string userId, string refreshToken)
    {
        var user = await _context.Users.FindAsync(userId);
        if(user==null) throw new CustomException(ErrorCode.UserNotfound);
        user.RefreshToken=refreshToken;        
        await _context.SaveChangesAsync();
    }

    public async Task<FullToken> BuildFullTokenAsync(ProjectUser user)
    {
        if (user != null)
        {
            var claimPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)claimPrincipal.Identity!;

            var newRefreshToken = GenerateRefreshToken();
            await ReplaceRefreshTokenAsync(user.Id, newRefreshToken);

            var newJwtToken = GenerateAccessToken(identity.Claims);

            return new FullToken
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }
        else
        {
            throw new ArgumentNullException("user");
        }
    } 
}

