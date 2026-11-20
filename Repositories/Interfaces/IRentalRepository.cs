using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IRentalRepository : IRepository<Rental>
    {
        Task<IEnumerable<Rental>> GetAllWithDetailsAsync();

        Task<Rental?> GetByIdWithDetailsAsync(int id);
    }
}
