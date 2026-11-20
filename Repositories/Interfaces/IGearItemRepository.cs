using HikeRent.Models;

namespace HikeRent.Repositories.Interfaces
{
    public interface IGearItemRepository : IRepository<GearItem>
    {
        Task<IEnumerable<GearItem>> GetAllWithDetailsAsync();

        Task<GearItem?> GetByIdWithDetailsAsync(int id);

        Task<bool> CategoryExistsAsync(int categoryId);

        Task<bool> BrandExistsAsync(int brandId);

        Task<int> GetRentedStockAsync(int gearItemId);

        Task<GearItem?> GetForStockUpdateAsync(int id);

    }
}
