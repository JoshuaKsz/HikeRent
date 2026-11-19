using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User> 
    {
        Task<User?> GetByEmailAsync(string email);

        Task<bool> EmailExistAsync(string email);
    }
}
