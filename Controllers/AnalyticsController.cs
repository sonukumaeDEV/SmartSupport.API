
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSupport.API.Data;

namespace SmartSupport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: api/Analytics/dashboard
        // ==========================================

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardAnalytics()
        {
            // ------------------------------------------
            // TOTAL COMPLAINTS
            // ------------------------------------------

            var totalComplaints =
                await _context.Complaints.CountAsync();


            // ------------------------------------------
            // STATUS COUNTS
            // ------------------------------------------

            var openComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Status == "Open");

            var assignedComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Status == "Assigned");

            var inProgressComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Status == "In Progress");

            var resolvedComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Status == "Resolved");

            var closedComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Status == "Closed");


            // ------------------------------------------
            // PRIORITY COUNTS
            // ------------------------------------------

            var highPriorityComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Priority == "High");

            var criticalPriorityComplaints =
                await _context.Complaints
                    .CountAsync(c => c.Priority == "Critical");


            // ------------------------------------------
            // CATEGORY STATISTICS
            // ------------------------------------------

            var categoryStats =
                await _context.Complaints
                    .Include(c => c.Category)
                    .GroupBy(c =>
                        c.Category != null
                            ? c.Category.Name
                            : "Uncategorized")
                    .Select(g => new
                    {
                        category = g.Key,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .ToListAsync();


            // ------------------------------------------
            // AGENT WORKLOAD
            // ------------------------------------------

            var agentWorkload =
                await _context.Complaints
                    .Where(c => c.AssignedAgentId != null)
                    .Include(c => c.AssignedAgent)
                    .GroupBy(c =>
                        c.AssignedAgent != null
                            ? c.AssignedAgent.Name
                            : "Unknown Agent")
                    .Select(g => new
                    {
                        agent = g.Key,
                        totalAssigned = g.Count(),

                        open = g.Count(
                            c => c.Status == "Open"),

                        assigned = g.Count(
                            c => c.Status == "Assigned"),

                        inProgress = g.Count(
                            c => c.Status == "In Progress"),

                        resolved = g.Count(
                            c => c.Status == "Resolved")
                    })
                    .OrderByDescending(x => x.totalAssigned)
                    .ToListAsync();


            // ------------------------------------------
            // SENTIMENT STATISTICS
            // ------------------------------------------

            var sentimentStats =
                await _context.Complaints
                    .Where(c =>
                        c.Sentiment != null &&
                        c.Sentiment != "")
                    .GroupBy(c => c.Sentiment)
                    .Select(g => new
                    {
                        sentiment = g.Key,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .ToListAsync();


            // ------------------------------------------
            // FINAL RESPONSE
            // ------------------------------------------

            return Ok(new
            {
                summary = new
                {
                    totalComplaints,

                    openComplaints,

                    assignedComplaints,

                    inProgressComplaints,

                    resolvedComplaints,

                    closedComplaints,

                    highPriorityComplaints,

                    criticalPriorityComplaints
                },

                categoryStats,

                agentWorkload,

                sentimentStats
            });
        }
    }
}

