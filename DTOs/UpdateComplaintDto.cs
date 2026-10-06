
using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.DTOs
{
    public class UpdateComplaintDto
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

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(
            "^(Open|Assigned|In Progress|Resolved|Closed)$",
            ErrorMessage =
                "Status must be Open, Assigned, In Progress, Resolved or Closed."
        )]
        public string Status { get; set; } = "Open";

        [Required(ErrorMessage = "Priority is required.")]
        [RegularExpression(
            "^(Low|Medium|High|Critical)$",
            ErrorMessage =
                "Priority must be Low, Medium, High or Critical."
        )]
        public string Priority { get; set; } = "Medium";

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
//    public class UpdateComplaintDto
//    {
//        [Required]
//        [MaxLength(200)]
//        public string Title { get; set; } = string.Empty;

//        [Required]
//        public string Description { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(30)]
//        public string Status { get; set; } = "Open";

//        [Required]
//        [MaxLength(30)]
//        public string Priority { get; set; } = "Medium";

//        public int? CategoryId { get; set; }
//    }
//}
