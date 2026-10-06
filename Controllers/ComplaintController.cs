using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSupport.API.Data;
using SmartSupport.API.DTOs;
using SmartSupport.API.Interfaces;
using SmartSupport.API.Models;
using System.Security.Claims;

namespace SmartSupport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IGeminiService _geminiService;

        public ComplaintController(
            AppDbContext context,
            IGeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        // ==========================================
        // GET ALL COMPLAINTS
        // SupportAgent + Admin
        // ==========================================
        [Authorize(Roles = "SupportAgent,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllComplaints()
        {
            var complaints = await _context.Complaints
                .Include(c => c.User)
                .Include(c => c.Category)
                .Include(c => c.AssignedAgent)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status,
                    Priority = c.Priority,
                    Sentiment = c.Sentiment,
                    AI_Category = c.AI_Category,
                    AI_Summary = c.AI_Summary,
                    AI_SuggestedResponse = c.AI_SuggestedResponse,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    UserId = c.UserId,
                    UserName = c.User != null ? c.User.Name : null,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null
                        ? c.Category.Name
                        : null,
                    AssignedAgentId = c.AssignedAgentId,
                    AssignedAgentName = c.AssignedAgent != null
                        ? c.AssignedAgent.Name
                        : null
                })
                .ToListAsync();

            return Ok(complaints);
        }

        // ==========================================
        // GET MY COMPLAINTS
        // ==========================================
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity"
                });
            }

            var complaints = await _context.Complaints
                .Include(c => c.User)
                .Include(c => c.Category)
                .Include(c => c.AssignedAgent)
                .Where(c => c.UserId == userId.Value)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status,
                    Priority = c.Priority,
                    Sentiment = c.Sentiment,
                    AI_Category = c.AI_Category,
                    AI_Summary = c.AI_Summary,
                    AI_SuggestedResponse = c.AI_SuggestedResponse,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    UserId = c.UserId,
                    UserName = c.User != null ? c.User.Name : null,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null
                        ? c.Category.Name
                        : null,
                    AssignedAgentId = c.AssignedAgentId,
                    AssignedAgentName = c.AssignedAgent != null
                        ? c.AssignedAgent.Name
                        : null
                })
                .ToListAsync();

            return Ok(complaints);
        }

        // ==========================================
        // GET ASSIGNED COMPLAINTS
        // SupportAgent
        // ==========================================
        [Authorize(Roles = "SupportAgent")]
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedComplaints()
        {
            var agentId = GetCurrentUserId();

            if (agentId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity"
                });
            }

            var complaints = await _context.Complaints
                .Include(c => c.User)
                .Include(c => c.Category)
                .Include(c => c.AssignedAgent)
                .Where(c => c.AssignedAgentId == agentId.Value)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status,
                    Priority = c.Priority,
                    Sentiment = c.Sentiment,
                    AI_Category = c.AI_Category,
                    AI_Summary = c.AI_Summary,
                    AI_SuggestedResponse = c.AI_SuggestedResponse,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    UserId = c.UserId,
                    UserName = c.User != null ? c.User.Name : null,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null
                        ? c.Category.Name
                        : null,
                    AssignedAgentId = c.AssignedAgentId,
                    AssignedAgentName = c.AssignedAgent != null
                        ? c.AssignedAgent.Name
                        : null
                })
                .ToListAsync();

            return Ok(complaints);
        }

        // ==========================================
        // GET SINGLE COMPLAINT
        // ==========================================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplaint(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.User)
                .Include(c => c.Category)
                .Include(c => c.AssignedAgent)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "Complaint not found"
                });
            }

            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity"
                });
            }

            var role = GetCurrentUserRole();

            if (role == "Customer" &&
                complaint.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            if (role == "SupportAgent" &&
                complaint.AssignedAgentId != currentUserId.Value)
            {
                return Forbid();
            }

            var response = new ComplaintResponseDto
            {
                Id = complaint.Id,
                Title = complaint.Title,
                Description = complaint.Description,
                Status = complaint.Status,
                Priority = complaint.Priority,
                Sentiment = complaint.Sentiment,
                AI_Category = complaint.AI_Category,
                AI_Summary = complaint.AI_Summary,
                AI_SuggestedResponse =
                    complaint.AI_SuggestedResponse,
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt,
                UserId = complaint.UserId,
                UserName = complaint.User?.Name,
                CategoryId = complaint.CategoryId,
                CategoryName = complaint.Category?.Name,
                AssignedAgentId = complaint.AssignedAgentId,
                AssignedAgentName =
                    complaint.AssignedAgent?.Name
            };

            return Ok(response);
        }

        // ==========================================
        // CREATE COMPLAINT + GEMINI AI
        // ==========================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComplaint(
            CreateComplaintDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity"
                });
            }

            var user = await _context.Users
                .FindAsync(userId.Value);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "User not found"
                });
            }

            // Validate category
            if (dto.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FindAsync(dto.CategoryId.Value);

                if (category == null)
                {
                    return BadRequest(new
                    {
                        message = "Category not found"
                    });
                }
            }

            // Create complaint
            var complaint = new Complaint
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = userId.Value,
                CategoryId = dto.CategoryId,
                Status = "Open",
                Priority = "Medium",
                CreatedAt = DateTime.UtcNow
            };

            _context.Complaints.Add(complaint);

            // Save first so complaint gets an ID
            await _context.SaveChangesAsync();

            // ==========================================
            // GEMINI AI ANALYSIS
            // ==========================================

            try
            {
                var aiResult =
                    await _geminiService.AnalyzeComplaintAsync(
                        complaint.Title,
                        complaint.Description
                    );

                complaint.AI_Category =
                    aiResult.Category;

                complaint.Priority =
                    aiResult.Priority;

                complaint.Sentiment =
                    aiResult.Sentiment;

                complaint.AI_Summary =
                    aiResult.Summary;

                complaint.AI_SuggestedResponse =
                    aiResult.SuggestedResponse;

                complaint.UpdatedAt =
                    DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Complaint remains saved even if AI fails

                return StatusCode(500, new
                {
                    message =
                        "Complaint created, but AI analysis failed.",
                    complaintId = complaint.Id,
                    error = ex.Message
                });
            }

            return CreatedAtAction(
                nameof(GetComplaint),
                new
                {
                    id = complaint.Id
                },
                new
                {
                    message =
                        "Complaint created and analyzed successfully",

                    complaintId = complaint.Id,

                    ai = new
                    {
                        category =
                            complaint.AI_Category,

                        priority =
                            complaint.Priority,

                        sentiment =
                            complaint.Sentiment,

                        summary =
                            complaint.AI_Summary,

                        suggestedResponse =
                            complaint.AI_SuggestedResponse
                    }
                }
            );
        }

        // ==========================================
        // UPDATE COMPLAINT
        // ==========================================
        [Authorize(Roles = "SupportAgent,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplaint(
            int id,
            UpdateComplaintDto dto)
        {
            var complaint = await _context.Complaints
                .FindAsync(id);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "Complaint not found"
                });
            }

            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity"
                });
            }

            var role = GetCurrentUserRole();

            if (role == "SupportAgent" &&
                complaint.AssignedAgentId != currentUserId.Value)
            {
                return Forbid();
            }

            if (dto.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FindAsync(dto.CategoryId.Value);

                if (category == null)
                {
                    return BadRequest(new
                    {
                        message = "Category not found"
                    });
                }
            }

            complaint.Title = dto.Title;
            complaint.Description = dto.Description;
            complaint.Status = dto.Status;
            complaint.Priority = dto.Priority;
            complaint.CategoryId = dto.CategoryId;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Complaint updated successfully",
                complaintId = complaint.Id
            });
        }

        // ==========================================
        // ASSIGN COMPLAINT
        // Admin only
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignComplaint(
            int id,
            int agentId)
        {
            var complaint = await _context.Complaints
                .FindAsync(id);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "Complaint not found"
                });
            }

            var agent = await _context.Users
                .FindAsync(agentId);

            if (agent == null)
            {
                return BadRequest(new
                {
                    message = "Agent not found"
                });
            }

            if (agent.Role != "SupportAgent")
            {
                return BadRequest(new
                {
                    message =
                        "Selected user is not a support agent"
                });
            }

            complaint.AssignedAgentId = agentId;
            complaint.Status = "Assigned";
            complaint.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Complaint assigned successfully",

                complaintId =
                    complaint.Id,

                assignedAgentId =
                    agentId,

                status =
                    complaint.Status
            });
        }

        // ==========================================
        // DELETE COMPLAINT
        // Admin only
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            var complaint = await _context.Complaints
                .FindAsync(id);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "Complaint not found"
                });
            }

            _context.Complaints.Remove(complaint);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Complaint deleted successfully"
            });
        }

        // ==========================================
        // GET CURRENT USER ID
        // ==========================================
        private int? GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(
                userIdClaim,
                out int userId))
            {
                return userId;
            }

            return null;
        }

        // ==========================================
        // GET CURRENT USER ROLE
        // ==========================================
        private string? GetCurrentUserRole()
        {
            return User.FindFirst(
                ClaimTypes.Role)?.Value;
        }
    }
}