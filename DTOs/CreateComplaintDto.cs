
using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.DTOs
{
    public class CreateComplaintDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(
            200,
            MinimumLength = 3,
            ErrorMessage = "Title must be between 3 and 200 characters."
        )]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(
            5000,
            MinimumLength = 10,
            ErrorMessage = "Description must be between 10 and 5000 characters."
        )]
        public string Description { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "CategoryId must be a valid category ID."
        )]
        public int? CategoryId { get; set; }
    }
}













//using System.ComponentModel.DataAnnotations;

//namespace SmartSupport.API.DTOs
//{
//    public class CreateComplaintDto
//    {
//        [Required]
//        [MaxLength(200)]
//        public string Title { get; set; } = string.Empty;

//        [Required]
//        public string Description { get; set; } = string.Empty;

//        public int? CategoryId { get; set; }
//    }
//}
