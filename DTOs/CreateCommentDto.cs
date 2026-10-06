using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
