using HikeRent.DTOs.Rentals;

namespace HikeRent.Services.Interfaces
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalResponse>> GetAllAsync();

        Task<RentalResponse?> GetByIdAsync(int id);

        Task<IEnumerable<RentalResponse>> GetReturnedUnpaidAsync();

        Task<RentalResponse> CreateAsync(RentalRequest request);
    }
}
