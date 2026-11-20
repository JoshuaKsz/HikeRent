using HikeRent.Data;
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
    }
}
