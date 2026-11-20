using HikeRent.Data;
using HikeRent.Enums;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class GearItemRepository : Repository<GearItem>, IGearItemRepository
    {
        public GearItemRepository(HikeRentDbContext context): base(context)
        {
        }

        public async Task<IEnumerable<GearItem>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(g => g.Category)
                .Include(g => g.Brand)
                .ToListAsync();
        }

        public async Task<GearItem?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(g => g.Category)
                .Include(g => g.Brand)
                .FirstOrDefaultAsync(g => g.GearItemId == id);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories
                .AnyAsync(c => c.CategoryId == categoryId);
        }

        public async Task<bool> BrandExistsAsync(int brandId)
        {
            return await _context.Brands
                .AnyAsync(b => b.BrandId == brandId);
        }


        public async Task<int> GetRentedStockAsync(int gearItemId)
        {
            return await _context.RentalDetails
                .Where(rd =>
                    rd.GearItemId == gearItemId &&
                    (rd.Rental!.Status == RentalStatus.Pending ||
                     rd.Rental.Status == RentalStatus.Ongoing))
                .SumAsync(rd => (int?)rd.Quantity) ?? 0;
        }

        public async Task<GearItem?> GetForStockUpdateAsync(int id)
        {
            if (_context.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException(
                    "Stock locking requires an active database transaction."
                );
            }

            return await _dbSet
                .FromSqlInterpolated(
                    $"SELECT * FROM \"GearItems\" WHERE \"GearItemId\" = {id} FOR UPDATE"
                )
                .FirstOrDefaultAsync();
        }

    }
}
