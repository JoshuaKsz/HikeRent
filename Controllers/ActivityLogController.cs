
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    }
}
