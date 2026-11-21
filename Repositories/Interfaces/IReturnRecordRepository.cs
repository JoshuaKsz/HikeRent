using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IReturnRecordRepository: IRepository<ReturnRecord>
    {
        //Task<IEnumerable<ReturnRecord>> GetAllWithDetailsAsync();

        //Task<ReturnRecord?> GetByIdWithDetailsAsync(int id);

        Task<ReturnRecord?> GetByRentalDetailIdAsync(int rentalDetailId);


    }
}
