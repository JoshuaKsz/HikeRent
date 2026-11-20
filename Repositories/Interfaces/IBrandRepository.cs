using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<bool> BrandNameExistsAsync(string brandName);
    }
}
