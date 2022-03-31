using System.ComponentModel.DataAnnotations;

namespace BmeModels
{
    public class UserLoginDTO
    {
        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        [RegularExpression(@"[A-Za-z0-9][^\>]*", ErrorMessage = "Password must contain at least one capital letter, one lowercase letter and one digit")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
