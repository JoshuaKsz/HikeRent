
using HikeRent.DTOs.ActivityLog;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HikeRent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        // GET: api/activitylog
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var activityLogs = await _activityLogService.GetAllAsync();

            return Ok(activityLogs);
        }

        // GET: api/activitylog/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var activityLog = await _activityLogService.GetByIdAsync(id);

            if (activityLog == null)
            {
                return NotFound(new
                {
                    message = "Data aktivitas tidak ditemukan."
                });
            }

            return Ok(activityLog);
        }

        // POST: api/activitylog
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ActivityLogRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "User ID tidak valid."
                });
            }

            var activityLog = await _activityLogService.CreateAsync(
                request,
                userId
            );

            return CreatedAtAction(
                nameof(GetById),
                new { id = activityLog.ActivityLogId },
                activityLog
            );
        }
    }
}
