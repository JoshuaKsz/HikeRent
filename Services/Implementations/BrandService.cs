using HikeRent.DTOs.Brands;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;

namespace HikeRent.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(
            IBrandRepository brandRepository,
            IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BrandResponse>> GetAllAsync()
        {
            IEnumerable<Brand> brands = await _brandRepository.GetAllAsync();

            return brands.Select(MapToResponse);
        }

        public async Task<BrandResponse?> GetByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                return null;
            }

            return MapToResponse(brand);
        }

        public async Task<BrandResponse> CreateAsync(BrandRequest request)
        {
            var brandName = request.BrandName.Trim();

            if (await _brandRepository.BrandNameExistsAsync(brandName))
            {
                throw new InvalidOperationException(
                    "Brand name already exists.");
            }

            var brand = new Brand
            {
                BrandName = brandName,
                Description = request.Description?.Trim()
            };

            await _brandRepository.AddAsync(brand);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(brand);
        }

        public async Task<BrandResponse?> UpdateAsync(
            int id,
            BrandRequest request)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                return null;
            }

            var brandName = request.BrandName.Trim();

            var isNameChanged = !string.Equals(
                brand.BrandName,
                brandName,
                StringComparison.OrdinalIgnoreCase);

            if (isNameChanged &&
                await _brandRepository.BrandNameExistsAsync(brandName))
            {
                throw new InvalidOperationException(
                    "Brand name already exists.");
            }

            brand.BrandName = brandName;
            brand.Description = request.Description?.Trim();

            _brandRepository.Update(brand);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(brand);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                return false;
            }

            _brandRepository.Delete(brand);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static BrandResponse MapToResponse(Brand brand)
        {
            return new BrandResponse
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description
            };
        }
    }
}
