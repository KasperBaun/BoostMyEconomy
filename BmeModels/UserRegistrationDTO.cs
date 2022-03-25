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
        [StringLength(20, ErrorMessage = "Name length can't be more than 20.")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be more than 20.")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
