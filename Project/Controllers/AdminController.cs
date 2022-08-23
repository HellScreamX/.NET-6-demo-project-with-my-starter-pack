using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogicLayer.Services;

[Produces("application/json")]
[Route("api/Admin")]
[ApiController]
[Authorize(Policy = "SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly UserManager<ProjectUser> _userManager;
    private readonly SignInManager<ProjectUser> _signInManager;
    private readonly IProjectUserManager _projectUserManager;
    private readonly ITokenManager _ITokenManager;
    private readonly ILogger _logger;
    private IConfiguration _config;

    public AdminController(IEmailService emailService,
    UserManager<ProjectUser> userManager, SignInManager<ProjectUser> signInManager,
     ILogger<AccountController> logger, IConfiguration config,
     ITokenManager ITokenManager, IProjectUserManager projectUserManager)
    {
        _emailService = emailService;
        _userManager = userManager;
        _signInManager = signInManager;
        _ITokenManager = ITokenManager;
        _logger = logger;
        _config = config;
        _projectUserManager = projectUserManager;
    }


    [HttpPost("CreateSub")]
    public async Task<IActionResult> CreateSubAdmin(string email, List<string> roles)
    {
        await _projectUserManager.CreateUserWithRoles(email, roles);
        return Ok();
    }
    [HttpDelete("DeleteSub/{email}")]
    public async Task<IActionResult> DeleteSubAdmin(string email)
    {
        await _projectUserManager.DeleteSubAdmin(email);
        return Ok();
    }

    [HttpGet("GetSubAdmins")]
    public async Task<IActionResult> GetSubAdmins()
    {
        var result = await _projectUserManager.GetAdministrators();
        return Ok(result);
    }
}



