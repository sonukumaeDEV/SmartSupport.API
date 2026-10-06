namespace SmartSupport.API.DTOs
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }

        public int ComplaintId { get; set; }
    }
}
