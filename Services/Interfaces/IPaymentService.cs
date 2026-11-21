using HikeRent.DTOs.Payments;

namespace HikeRent.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponse>> GetAllAsync();

        Task<PaymentResponse?> GetByRentalIdAsync(int rentalId);

        Task<PaymentResponse?> CreateAsync(int rentalId, PaymentRequest request);
    }
}
