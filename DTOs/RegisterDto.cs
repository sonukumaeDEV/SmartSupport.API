
using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters."
        )]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(
            150,
            ErrorMessage = "Email cannot exceed 150 characters."
        )]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 100 characters."
        )]
        public string Password { get; set; } = string.Empty;
    }
}












//using System.ComponentModel.DataAnnotations;

//namespace SmartSupport.API.DTOs
//{
//    public class RegisterDto
//    {
//        [Required]
//        [MaxLength(100)]
//        public string Name { get; set; } = string.Empty;

//        [Required]
//        [EmailAddress]
//        [MaxLength(150)]
//        public string Email { get; set; } = string.Empty;

//        [Required]
//        [MinLength(6)]
//        public string Password { get; set; } = string.Empty;
//    }
//}
