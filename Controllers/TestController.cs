using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartSupport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // ========================================
        // PUBLIC
        // ========================================

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new
            {
                message = "This is a public API"
            });
        }


        // ========================================
        // AUTHENTICATED USER
        // ========================================

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok(new
            {
                message = "You are authenticated!",
                user = User.Identity?.Name,
                email = User.FindFirst(
                    ClaimTypes.Email)?.Value,
                role = User.FindFirst(
                    ClaimTypes.Role)?.Value
            });
        }


        // ========================================
        // CUSTOMER ONLY
        // ========================================

        [Authorize(Roles = "Customer")]
        [HttpGet("customer")]
        public IActionResult CustomerOnly()
        {
            return Ok(new
            {
                message = "Customer access granted!",
                role = User.FindFirst(
                    ClaimTypes.Role)?.Value
            });
        }


        // ========================================
        // SUPPORT AGENT ONLY
        // ========================================

        [Authorize(Roles = "SupportAgent")]
        [HttpGet("support-agent")]
        public IActionResult SupportAgentOnly()
        {
            return Ok(new
            {
                message = "Support Agent access granted!",
                role = User.FindFirst(
                    ClaimTypes.Role)?.Value
            });
        }


        // ========================================
        // ADMIN ONLY
        // ========================================

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new
            {
                message = "Admin access granted!",
                role = User.FindFirst(
                    ClaimTypes.Role)?.Value
            });
        }
    }
}