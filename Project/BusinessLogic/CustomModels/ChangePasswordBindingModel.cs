using System.ComponentModel.DataAnnotations;
#nullable disable
namespace Project.BusinessLogicLayer.CustomModels
{
    public class ChangePasswordBindingModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)] 
        public string NewPassword { get; set; }
        
    }
}
