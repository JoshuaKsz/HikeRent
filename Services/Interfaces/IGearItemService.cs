using HikeRent.DTOs.GearItems;

namespace HikeRent.Services.Interfaces
{
    public interface IGearItemService
    {
        Task<IEnumerable<GearItemResponse>> GetAllAsync();

        Task<GearItemResponse?> GetByIdAsync(int id);

        Task<GearItemResponse> CreateAsync(GearItemRequest request);

        Task<GearItemResponse?> UpdateAsync(int id, GearItemRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
