using HikeRent.DTOs.ActivityLog;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HikeRent.Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(
            IActivityLogRepository activityLogRepository,
            IUnitOfWork unitOfWork,
            ILogger<ActivityLogService> logger)
        {
            _activityLogRepository = activityLogRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<ActivityLogResponse>> GetAllAsync()
        {
            IEnumerable<ActivityLog> activityLogs =
                await _activityLogRepository.GetAllWithUserAsync();

            return activityLogs.Select(activityLog => new ActivityLogResponse
            {
                ActivityLogId = activityLog.ActivityLogId,
                UserId = activityLog.UserId,
                StaffName = activityLog.User?.FullName ?? string.Empty,
                Activity = activityLog.Activity,
                ActivityDate = activityLog.ActivityDate
            });
        }

        public async Task<ActivityLogResponse?> GetByIdAsync(int id)
        {
            ActivityLog? activityLog =
                await _activityLogRepository.GetByIdWithUserAsync(id);

            if (activityLog == null)
            {
                return null;
            }

            return new ActivityLogResponse
            {
                ActivityLogId = activityLog.ActivityLogId,
                UserId = activityLog.UserId,
                StaffName = activityLog.User?.FullName ?? string.Empty,
                Activity = activityLog.Activity,
                ActivityDate = activityLog.ActivityDate
            };
        }

        public async Task<ActivityLogResponse> CreateAsync(
            ActivityLogRequest request,
            int userId)
        {
            ActivityLog activityLog = new ActivityLog
            {
                UserId = userId,
                Activity = request.Activity
            };

            await _activityLogRepository.AddAsync(activityLog);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Staff {UserId} melakukan aktivitas: {Activity}", userId, request.Activity);

            ActivityLog? savedActivityLog =
                await _activityLogRepository.GetByIdWithUserAsync(
                    activityLog.ActivityLogId);

            return new ActivityLogResponse
            {
                ActivityLogId = activityLog.ActivityLogId,
                UserId = activityLog.UserId,
                StaffName = savedActivityLog?.User?.FullName ?? string.Empty,
                Activity = activityLog.Activity,
                ActivityDate = activityLog.ActivityDate
            };
        }
    }
}