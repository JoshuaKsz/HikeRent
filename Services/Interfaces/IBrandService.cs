using HikeRent.DTOs.Brands;

namespace HikeRent.Services.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandResponse>> GetAllAsync();

        Task<BrandResponse?> GetByIdAsync(int id);

        Task<BrandResponse> CreateAsync(BrandRequest request);

        Task<BrandResponse?> UpdateAsync(int id, BrandRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
