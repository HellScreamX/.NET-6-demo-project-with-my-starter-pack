
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ndif.BusinessLogicLayer.CustomModels;
using Project.BusinessLogic.customModels;
using Project.BusinessLogicLayer.CustomModels;
using Project.BusinessLogicLayer.Services;
using Project.ExceptionLayer;

[Produces("application/json")]
[Route("api/Account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly UserManager<ProjectUser> _userManager;
    private readonly SignInManager<ProjectUser> _signInManager;
    private readonly IProjectUserManager _projectUserManager;
    private readonly ITokenManager _ITokenManager;
    private readonly ILogger _logger;
    private IConfiguration _config;

    public AccountController(IEmailService emailService,
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterBindingModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var aspUser = new ProjectUser() { UserName = model.Email, Email = model.Email, EmailConfirmed = false};
        aspUser.CreationDate = DateTime.UtcNow;

        var code = await _projectUserManager.GenerateConfirmationCodeAsync(aspUser, saveChanges: false);
        aspUser.Code = code;
        IdentityResult result = await _userManager.CreateAsync(aspUser, model.Password);

        if (!result.Succeeded) throw new CustomException(ErrorManager.GetErrorCode(result));
        var addingRoleResult = await _projectUserManager.GiveRoleToUserAsync(aspUser, Roles.SuperAdmin);
        if (!addingRoleResult.Succeeded)
        {
            return BadRequest(addingRoleResult.Errors);
        }

        //=================SENDING CONFIRMATION EMAIL
        await SendConfirmationCodeAsync(aspUser);

        return Ok(result);
    }
    [HttpPost("registerContractor")]
    public async Task<IActionResult> RegisterContractor([FromBody] RegisterBindingModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var aspUser = new Contractor() { UserName = model.Email, Email = model.Email, EmailConfirmed = false};
        aspUser.CreationDate = DateTime.UtcNow;

        var code = await _projectUserManager.GenerateConfirmationCodeAsync(aspUser, saveChanges: false);
        aspUser.Code = code;
        IdentityResult result = await _userManager.CreateAsync(aspUser, model.Password);

        if (!result.Succeeded) throw new CustomException(ErrorManager.GetErrorCode(result));
        var addingRoleResult = await _projectUserManager.GiveRoleToUserAsync(aspUser, Roles.Contractor);
        if (!addingRoleResult.Succeeded)
        {
            return BadRequest(addingRoleResult.Errors);
        }

        //=================SENDING CONFIRMATION EMAIL
        await SendConfirmationCodeAsync(aspUser);

        return Ok(result);
    }

    [HttpGet("ReSendConfirmationCode")]
    public async Task<IActionResult> ReSendConfirmationCode(string email)
    {
        var user = await _projectUserManager.GetUserByUserNameAsync(email);

        if (user.EmailConfirmed == true) return Ok();

        await _projectUserManager.GenerateConfirmationCodeAsync(user, saveChanges: true);

        await _emailService.SendEmailSendGridAPIAsync(user.Email, _config["SendGrid:ConfirmTemplate"], new { email = user.Email, code = user.Code });

        return Ok();
    }

    [Authorize]
    [HttpGet("SendEditConfirmationCode")]
    public async Task<IActionResult> SendEditConfirmationCode()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        await _projectUserManager.GenerateConfirmationCodeAsync(user, saveChanges: true);

        await _emailService.SendEmailSendGridAPIAsync(user.Email, _config["SendGrid:ConfirmTemplate"], new { email = user.Email, code = user.Code });

        return Ok();
    }


    async Task SendConfirmationCodeAsync(ProjectUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        await _emailService.SendEmailSendGridAPIAsync(user.Email, _config["SendGrid:ConfirmTemplate"], new { email = user.Email, code = user.Code });

    }


    [Authorize]
    [HttpPost]
    [Route("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordBindingModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            throw new CustomException(ErrorManager.GetErrorCode(result));

        }
        return Ok();
    }


    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);

        //adding user infos settings

        return new ObjectResult(new
        {
            roles = roles,
            username = user.UserName,
            email = user.Email
        });
    }


    [HttpGet("ConfirmUser/{email}")]
    [Authorize(Policy = "admin")]
    public async Task<IActionResult> ConfirmUser(String email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new CustomException(ErrorCode.UserNotfound);
        }
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _userManager.ConfirmEmailAsync(user, code);
        return Ok();
    }


    [HttpGet("ConfirmEmailCode")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailCodeAsync(string email, string code)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _projectUserManager.ConfirmEmailCodeAsync(email, code);

        return Ok();
    }


    [HttpPost("ForgotPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _projectUserManager.GetUserByUserNameAsync(model.Email);

        //generate code
        await _projectUserManager.GenerateConfirmationCodeAsync(user, saveChanges: true);

        //send the email
        await _emailService.SendEmailSendGridAPIAsync(user.Email, _config["SendGrid:ForgotTemplate"], new { email = user.Email, code = user.Code });

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _projectUserManager.ResetPasswordAsync(model.Email, model.Code, model.Password);

        return Ok();
    }


    [HttpPost]
    [Authorize]
    [Route("RequestEditCode")]
    public async Task<IActionResult> RequestEditCode([FromBody] LoginModel login)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

        if (result.Succeeded)
        {
            var code = await _projectUserManager.GenerateConfirmationCodeAsync(user, saveChanges: true);
            return Ok(code);
        }
        else
        {
            throw new CustomException(ErrorCode.InvalidCredentials);
        }
    }
}



