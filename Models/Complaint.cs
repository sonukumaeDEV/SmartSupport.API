using System.ComponentModel.DataAnnotations;

namespace SmartSupport.API.Models
{
    public class Complaint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Status { get; set; } = "Open";

        [MaxLength(30)]
        public string Priority { get; set; } = "Medium";

        [MaxLength(30)]
        public string? Sentiment { get; set; }

        [MaxLength(100)]
        public string? AI_Category { get; set; }

        public string? AI_Summary { get; set; }

        public string? AI_SuggestedResponse { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Customer
        public int UserId { get; set; }
        public User? User { get; set; }

        // Category
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Assigned Support Agent
        public int? AssignedAgentId { get; set; }
        public User? AssignedAgent { get; set; }

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}