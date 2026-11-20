using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IActivityLogRepository : IRepository<ActivityLog>
    {
        Task<IEnumerable<ActivityLog>> GetAllWithUserAsync();

        Task<ActivityLog?> GetByIdWithUserAsync(int id);
    }
}
