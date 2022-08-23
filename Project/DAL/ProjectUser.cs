using Microsoft.AspNetCore.Identity;


public class ProjectUser : IdentityUser
{
    public string? Name { get; set; }
    public DateTime CreationDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime TokenExpire { get; set; }

    public string? Code { get; set; }
    public DateTime CodeExpiryDate { get; set; }

}