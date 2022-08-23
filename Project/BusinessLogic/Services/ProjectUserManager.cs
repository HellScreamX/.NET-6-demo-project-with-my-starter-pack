using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.DAL;
using Project.ExceptionLayer;
using Project.Utilities;

namespace Project.BusinessLogicLayer.Services
{
    public interface IProjectUserManager
    {
        Task<ProjectUser> GetUserByUserNameAsync(string userName);
        Task ConfirmEmailCodeAsync(string Email, string code);
        //Task ValidateUserAsync(string userId);
        Task<IdentityResult> GiveRoleToUserAsync(ProjectUser user, string role);
        Task ResetUserRolesAsync(ProjectUser user, List<string> roles);
        Task ResetPasswordAsync(string email, string code, string password);
        Task<string> GenerateConfirmationCodeAsync(ProjectUser user, bool saveChanges);
        bool IsValidCode(ProjectUser user, string code);
        Task CreateUserWithRoles(string email, List<string> roles);
        Task DeleteSubAdmin(string email);        
        Task<List<AdminWithRoles>> GetAdministrators();
    }
    public class ProjectUserManager : IProjectUserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ProjectUser> _userManager;
        private readonly IEmailService _emailService;
        private IConfiguration _config;
        private readonly IMapper _mapper;

        public ProjectUserManager(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ProjectUser> userManager, IEmailService emailService, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
            _mapper = mapper;
        }

        public async Task<ProjectUser> GetUserByUserNameAsync(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentException("userName is empty");
            var Client = await _context.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync()
            ?? throw new CustomException(ErrorCode.UserNotfound);
            return Client;
        }


        public async Task ConfirmEmailCodeAsync(string Email, string code)
        {
            var user = await GetUserByUserNameAsync(Email)
                ?? throw new CustomException(ErrorCode.UserNotfound);

            //check if the code provided is correct and not expired
            if (IsValidCode(user, code))
            {
                //set property to confirmed
                user.EmailConfirmed = true;

                //remove the current code
                user.Code = null;
                user.CodeExpiryDate = DateTime.MinValue;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CustomException(ErrorCode.InvalidCode);
            }
        }

        public bool IsValidCode(ProjectUser user, string code)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return (code == user.Code && user.CodeExpiryDate >= DateTime.Now);
        }

      

        public async Task<IdentityResult> GiveRoleToUserAsync(ProjectUser user, string role)
        {
            var roleCheck = await _roleManager.RoleExistsAsync(role);
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task ResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await GetUserByUserNameAsync(email)
                ?? throw new CustomException(ErrorCode.UserNotfound);

            //check if the code provided is correct and not expired
            if (IsValidCode(user, code))
            {
                //remove old password
                var result = await _userManager.RemovePasswordAsync(user);

                if (!result.Succeeded)
                    throw new CustomException(ErrorManager.GetErrorCode(result));

                //set the new password
                result = await _userManager.AddPasswordAsync(user, newPassword);

                if (!result.Succeeded)
                    throw new CustomException(ErrorManager.GetErrorCode(result));

                //remove the current code
                user.Code = null;
                user.CodeExpiryDate = DateTime.MinValue;

                user.EmailConfirmed = true;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CustomException(ErrorCode.InvalidCode);
            }
        }

        public async Task<string> GenerateConfirmationCodeAsync(ProjectUser user, bool saveChanges)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            int codeSize = 4; //size of the generated code
            int timeToExpire = 30; //minutes 

            char[] chars = "1234567890".ToCharArray();

            user.Code = Tools.GenerateRandomString(codeSize, chars);
            user.CodeExpiryDate = DateTime.Now.AddMinutes(timeToExpire);

            if (saveChanges)
                await _context.SaveChangesAsync();

            return user.Code;
        }

        

        public async Task CreateUserWithRoles(string email, List<string> roles)
        {
            List<string> allowedRoles = new List<string>
            {"PiecesManager", "CommandesManager", "DataAnalyst", "CarsManager"};
            foreach (var item in roles)
            {
                if (!allowedRoles.Contains(item))
                {
                    throw new CustomException("the role_" + item + "_ is not allowed");
                }
            }

            var administrator = await _userManager.FindByEmailAsync(email);
            //if the actual employee doesnt exist , we create a new account
            if (administrator == null)
            {
                //creating the admin account
                var password = Password.GeneratePassword(14, 1, 1, 1, 2);
                administrator = new Administrator() { UserName = email, Email = email, EmailConfirmed = true };
                administrator.CreationDate = DateTime.Now;
                IdentityResult result = await _userManager.CreateAsync(administrator, password);


                if (!result.Succeeded) throw new CustomException(ErrorManager.GetErrorCode(result));
                //adding the new roles
                await ResetUserRolesAsync(administrator, roles);
                //sending email to the new subAdmin
                await _emailService.SendEmailSendGridAPIAsync(email, _config["SendGrid:DasAutoNewAdmin"]
                                                              , new
                                                              {    
                                                                  name = email,
                                                                  link = _config["FrontPage:AdminUrl"],
                                                                  password = password
                                                              });

            }
            else
            {
                //adding the new roles
                await ResetUserRolesAsync(administrator, roles);
            }

        }

        public async Task ResetUserRolesAsync(ProjectUser user, List<string> rolesToAdd)
        {

            var oldUserRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldUserRoles);
            foreach (var item in rolesToAdd)
            {
                var roleCheck = await _roleManager.RoleExistsAsync(item);
                if (!roleCheck)
                {
                    //create the roles and seed them to the database
                    await _roleManager.CreateAsync(new IdentityRole(item));
                }
                var addingRoleResult = await _userManager.AddToRoleAsync(user, item);
                if (!addingRoleResult.Succeeded)
                {
                    throw new CustomException(ErrorManager.GetErrorCode(addingRoleResult));
                }
            }
        }

        public async Task<List<AdminWithRoles>> GetAdministrators()
        {
            var administrators = await _context.Administrators.ToListAsync();
            var adminWithRoles= new List<AdminWithRoles>();
            foreach (var item in administrators)
            {
                var roles =await _userManager.GetRolesAsync(item);
                adminWithRoles.Add(new AdminWithRoles{
                    Email=item.Email,
                    Roles = roles.ToList()
                });
            }
            return adminWithRoles;
        }

        

        public async Task DeleteSubAdmin(string email)
        {
            var userToDelete = await _userManager.FindByEmailAsync(email);
            IdentityResult result = await _userManager.DeleteAsync(userToDelete);

            if (!result.Succeeded) throw new CustomException(ErrorManager.GetErrorCode(result));
        }


    }
}


