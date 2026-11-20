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

        public async Task<Dictionary<int, int>> GetRentedStockByGearItemAsync(
    IEnumerable<int> gearItemIds)
        {
            var ids = gearItemIds.Distinct().ToArray();

            if (ids.Length == 0)
            {
                return new Dictionary<int, int>();
            }

            return await _context.RentalDetails
                .Where(rd =>
                    ids.Contains(rd.GearItemId) &&
                    (rd.Rental!.Status == RentalStatus.Pending ||
                     rd.Rental.Status == RentalStatus.Ongoing))
                .GroupBy(rd => rd.GearItemId)
                .Select(group => new
                {
                    GearItemId = group.Key,
                    Quantity = group.Sum(rd => rd.Quantity)
                })
                .ToDictionaryAsync(
                    x => x.GearItemId,
                    x => x.Quantity
                );
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

        public async Task<bool> HasRentalHistoryAsync(int gearItemId)
        {
            return await _context.RentalDetails
                .AnyAsync(rd => rd.GearItemId == gearItemId);
        }

        public async Task<bool> HasActiveRentalAsync(int gearItemId)
        {
            return await _context.RentalDetails
                .AnyAsync(rd =>
                    rd.GearItemId == gearItemId &&
                    (rd.Rental!.Status == RentalStatus.Pending ||
                     rd.Rental.Status == RentalStatus.Ongoing));
        }

    }
}
