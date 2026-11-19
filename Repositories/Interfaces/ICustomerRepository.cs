using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task<Customer?> GetByNameAsync(string fullName);

        Task<bool> IdentityNumberExistsAsync(string identityNumber);

    }
}
