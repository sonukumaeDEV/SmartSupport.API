using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
