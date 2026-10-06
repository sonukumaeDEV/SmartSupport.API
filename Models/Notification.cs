using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User? User { get; set; }

        public int? ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }
    }
}