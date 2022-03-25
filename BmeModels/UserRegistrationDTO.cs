using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmeModels
{
    public class UserRegistrationDTO
    {
        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be more than 20")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be more than 20")]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 and at most 30 characters long.", MinimumLength = 8)]
        [RegularExpression(@"[A-Za-z0-9][^\>]*", ErrorMessage = "Password must contain at least one capital letter, one lowercase letter and one digit")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string Email { get; set; } = string.Empty;
    }
}
