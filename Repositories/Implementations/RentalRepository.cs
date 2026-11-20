using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class RentalRepository : Repository<Rental>, IRentalRepository
    {
        public RentalRepository(HikeRentDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Rental>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.RentalDetails)
                    .ThenInclude(rd => rd.GearItem)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Rental?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.RentalDetails)
                    .ThenInclude(rd => rd.GearItem)
                .FirstOrDefaultAsync(r => r.RentalId == id);
        }

        public async Task<Rental?> GetByIdWithDetailsForUpdateAsync(int id)
        {
            if (_context.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException(
                    "Rental locking requires an active database transaction."
                );
            }

            return await _dbSet
                .FromSqlInterpolated(
                    $"SELECT * FROM \"Rentals\" WHERE \"RentalId\" = {id} FOR UPDATE"
                )
                .Include(r => r.RentalDetails)
                    .ThenInclude(rd => rd.ReturnRecord)
                .FirstOrDefaultAsync();
        }
    }
}
