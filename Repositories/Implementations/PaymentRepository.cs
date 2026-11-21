using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(HikeRentDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.Customer)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetByRentalIdWithDetailsAsync(int rentalId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.Customer)
                .FirstOrDefaultAsync(p => p.RentalId == rentalId);
        }
    }
}
