using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string categoryName);

        Task<bool> CategoryNameExistsAsync(string categoryName);

    }
}
