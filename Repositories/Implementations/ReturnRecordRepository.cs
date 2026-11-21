using HikeRent.Data;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Repositories.Implementations
{
    public class ReturnRecordRepository : Repository<ReturnRecord>, IReturnRecordRepository
    {
        public ReturnRecordRepository(HikeRentDbContext context) : base(context)
        {
        }

        //public async Task<IEnumerable<ReturnRecord>> GetAllWithDetailsAsync()
        //{
        //    return await _dbSet
        //        .AsNoTracking()
        //        .Include(rr => rr.User)
        //        .Include(rr => rr.RentalDetail)
        //            .ThenInclude(rd => rd.Rental)
        //                .ThenInclude(r => r.Customer)
        //        .Include(rr => rr.RentalDetail)
        //            .ThenInclude(rd => rd.GearItem)
        //        .OrderByDescending(rr => rr.ActualReturnDate)
        //        .ToListAsync();
        //}

        //public async Task<ReturnRecord?> GetByIdWithDetailsAsync(int id)
        //{
        //    return await _dbSet
        //        .AsNoTracking()
        //        .Include(rr => rr.User)
        //        .Include(rr => rr.RentalDetail)
        //            .ThenInclude(rd => rd.Rental)
        //                .ThenInclude(r => r.Customer)
        //        .Include(rr => rr.RentalDetail)
        //            .ThenInclude(rd => rd.GearItem)
        //        .FirstOrDefaultAsync(rr => rr.ReturnRecordId == id);
        //}

        public async Task<ReturnRecord?> GetByRentalDetailIdAsync(int rentalDetailId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(rr => rr.User)
                .Include(rr => rr.RentalDetail)
                    .ThenInclude(rd => rd.Rental)
                .Include(rr => rr.RentalDetail)
                    .ThenInclude(rd => rd.GearItem)
                .FirstOrDefaultAsync(
                    rr => rr.RentalDetailId == rentalDetailId
                );
        }
    }

}
