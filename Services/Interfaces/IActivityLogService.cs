
using HikeRent.DTOs.ActivityLog;

namespace HikeRent.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task<IEnumerable<ActivityLogResponse>> GetAllAsync();

        Task<ActivityLogResponse?> GetByIdAsync(int id);

        Task<ActivityLogResponse> CreateAsync(
            ActivityLogRequest request,
            int userId);
    }
}
