
using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters."
        )]
        public string Password { get; set; } = string.Empty;
    }
}









//using System.ComponentModel.DataAnnotations;

//namespace SmartSupport.API.DTOs
//{
//    public class LoginDto
//    {
//        [Required]
//        [EmailAddress]
//        public string Email { get; set; } = string.Empty;

//        [Required]
//        public string Password { get; set; } = string.Empty;
//    }
//}