using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(HikeRentDbContext context) : base(context) 
        {

        }
        public async Task<Customer?> GetByNameAsync(string fullName)
        {
            return await _dbSet
              .FirstOrDefaultAsync(c => c.FullName.ToLower() == fullName.Trim().ToLower());
        }

        public async Task<bool> IdentityNumberExistsAsync(string identityNumber)
        {
            return await _dbSet
                .AnyAsync(c => identityNumber.ToLower() == c.IdentityNumber.Trim().ToLower());
        }
    }
}
