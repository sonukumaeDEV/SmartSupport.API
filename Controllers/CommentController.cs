using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSupport.API.Data;
using SmartSupport.API.DTOs;
using SmartSupport.API.Models;
using System.Security.Claims;

namespace SmartSupport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetComments(int complaintId)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == complaintId);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "Complaint not found"
                });
            }

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            if (userId == null)
            {
                return Unauthorized();
            }

            if (role == "Customer" &&
                complaint.UserId != userId.Value)
            {
                return Forbid();
            }

            if (role == "SupportAgent" &&
                complaint.AssignedAgentId != userId.Value)
            {
                return Forbid();
            }

            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ComplaintId == complaintId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User != null ? c.User.Name : null,
                    ComplaintId = c.ComplaintId
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost("{complaintId}")]
        public async Task<IActionResult> AddComment(
            int complaintId,
            CreateCommentDto dto)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == complaintId);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "Complaint not found"
                });
            }

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            if (userId == null)
            {
                return Unauthorized();
            }

            if (role == "Customer" &&
                complaint.UserId != userId.Value)
            {
                return Forbid();
            }

            if (role == "SupportAgent" &&
                complaint.AssignedAgentId != userId.Value)
            {
                return Forbid();
            }

            var comment = new Comment
            {
                Message = dto.Message,
                ComplaintId = complaintId,
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            // Customer comment → notify assigned agent
            if (role == "Customer" &&
                complaint.AssignedAgentId.HasValue)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = complaint.AssignedAgentId.Value,
                    ComplaintId = complaint.Id,
                    Message = $"New comment added to complaint #{complaint.Id}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Agent comment → notify customer
            if (role == "SupportAgent")
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = complaint.UserId,
                    ComplaintId = complaint.Id,
                    Message = $"Support agent replied to complaint #{complaint.Id}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Comment added successfully",
                commentId = comment.Id
            });
        }

        private int? GetCurrentUserId()
        {
            var value = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(value, out int userId))
            {
                return userId;
            }

            return null;
        }

        private string? GetCurrentUserRole()
        {
            return User.FindFirst(
                ClaimTypes.Role)?.Value;
        }
    }
}
