using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Project.BusinessLogic.customModels
{
    public class RegisterBindingModel
	{
		[Required]		
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		public string Password { get; set; }
	}
}
