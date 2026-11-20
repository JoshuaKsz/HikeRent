using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(HikeRentDbContext context)
            : base(context)
        {
        }

        public async Task<bool> BrandNameExistsAsync(string brandName)
        {
            return await _dbSet.AnyAsync(b =>
                b.BrandName.ToLower() == brandName.ToLower());
        }
    }
}
