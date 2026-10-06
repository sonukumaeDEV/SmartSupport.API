namespace SmartSupport.API.DTOs
{
    public class ComplaintResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string? Sentiment { get; set; }

        public string? AI_Category { get; set; }

        public string? AI_Summary { get; set; }

        public string? AI_SuggestedResponse { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? AssignedAgentId { get; set; }

        public string? AssignedAgentName { get; set; }
    }
}
