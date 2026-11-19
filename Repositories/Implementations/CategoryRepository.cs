using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(HikeRentDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.Trim().ToLower());
        }

        public async Task<bool> CategoryNameExistsAsync(string categoryName)
        {
            return await _dbSet
                .AnyAsync(c => categoryName.ToLower() == c.CategoryName.Trim().ToLower());
        }
    }
}
