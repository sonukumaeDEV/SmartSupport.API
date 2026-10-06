namespace SmartSupport.API.DTOs
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? ComplaintId { get; set; }
    }
}
