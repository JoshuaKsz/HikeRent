
using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(HikeRentDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<ActivityLog>> GetAllWithUserAsync()
        {
            return await _dbSet
                .Include(a => a.User)
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync();
        }

        public async Task<ActivityLog?> GetByIdWithUserAsync(int id)
        {
            return await _dbSet
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ActivityLogId == id);
        }
    }
}
