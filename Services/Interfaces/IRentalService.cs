using HikeRent.DTOs.Rentals;

namespace HikeRent.Services.Interfaces
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalResponse>> GetAllAsync();

        Task<RentalResponse?> GetByIdAsync(int id);

        Task<RentalResponse> CreateAsync(RentalRequest request);

        Task<RentalResponse?> UpdateStatusAsync(int id, string status);

        Task<bool> CancelAsync(int id);
    }
}
