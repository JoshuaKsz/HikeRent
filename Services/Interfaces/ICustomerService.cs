using HikeRent.DTOs.Customer;

namespace HikeRent.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponse>> GetAllAsync();

        Task<CustomerResponse?> GetByIdAsync(int id);

        Task<CustomerResponse> CreateAsync(CustomerRequest request);

        Task<CustomerResponse?> UpdateAsync(int id, CustomerRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
