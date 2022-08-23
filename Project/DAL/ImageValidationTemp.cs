
#nullable disable

using System.ComponentModel.DataAnnotations;

namespace Project.DAL
{
    public class ImageValidationTemp
    {
        [Key]
        public string FileName { get; set; }
        [Required]
        public string PresignedUrl { get; set; }
    }
}
