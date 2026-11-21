using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetAllWithDetailsAsync();

        Task<Payment?> GetByRentalIdWithDetailsAsync(int rentalId);
    }
}
