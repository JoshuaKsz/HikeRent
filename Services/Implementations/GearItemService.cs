using HikeRent.DTOs.GearItems;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;

namespace HikeRent.Services.Implementations
{
    public class GearItemService : IGearItemService
    {
        private readonly IGearItemRepository _gearItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GearItemService(
            IGearItemRepository gearItemRepository,
            IUnitOfWork unitOfWork)
        {
            _gearItemRepository = gearItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GearItemResponse>> GetAllAsync()
        {
            IEnumerable<GearItem> gearItems =
                await _gearItemRepository.GetAllWithDetailsAsync();

            return gearItems.Select(MapToResponse);
        }

        public async Task<GearItemResponse?> GetByIdAsync(int id)
        {
            GearItem? gearItem =
                await _gearItemRepository.GetByIdWithDetailsAsync(id);

            if (gearItem == null)
            {
                return null;
            }

            return MapToResponse(gearItem);
        }

        public async Task<GearItemResponse> CreateAsync(
            GearItemRequest request)
        {
            await ValidateReferencesAsync(request);

            ValidateBusinessRules(request);

            GearItem gearItem = new GearItem
            {
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                GearName = request.GearName.Trim(),
                Description = request.Description?.Trim(),
                DailyPrice = request.DailyPrice,
                ReplacementPrice = request.ReplacementPrice,
                TotalStock = request.TotalStock,
                MaintenanceStock = request.MaintenanceStock,
                DamagedStock = request.DamagedStock,
                ImageUrl = request.ImageUrl?.Trim(),
                IsActive = request.IsActive
            };

            await _gearItemRepository.AddAsync(gearItem);

            await _unitOfWork.SaveChangesAsync();

            GearItem? savedGearItem =
                await _gearItemRepository.GetByIdWithDetailsAsync(gearItem.GearItemId);

            return MapToResponse(savedGearItem!);
        }

        public async Task<GearItemResponse?> UpdateAsync(
            int id,
            GearItemRequest request)
        {
            GearItem? gearItem =
                await _gearItemRepository.GetByIdAsync(id);

            if (gearItem == null)
            {
                return null;
            }

            await ValidateReferencesAsync(request);

            ValidateBusinessRules(request);

            gearItem.CategoryId = request.CategoryId;
            gearItem.BrandId = request.BrandId;
            gearItem.GearName = request.GearName.Trim();
            gearItem.Description = request.Description?.Trim();
            gearItem.DailyPrice = request.DailyPrice;
            gearItem.ReplacementPrice = request.ReplacementPrice;
            gearItem.TotalStock = request.TotalStock;
            gearItem.MaintenanceStock = request.MaintenanceStock;
            gearItem.DamagedStock = request.DamagedStock;
            gearItem.ImageUrl = request.ImageUrl?.Trim();
            gearItem.IsActive = request.IsActive;

            _gearItemRepository.Update(gearItem);

            await _unitOfWork.SaveChangesAsync();

            GearItem? updatedGearItem =
                await _gearItemRepository.GetByIdWithDetailsAsync(id);

            return MapToResponse(updatedGearItem!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            GearItem? gearItem =
                await _gearItemRepository.GetByIdAsync(id);

            if (gearItem == null)
            {
                return false;
            }

            _gearItemRepository.Delete(gearItem);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task ValidateReferencesAsync(
            GearItemRequest request)
        {
            if (!await _gearItemRepository.CategoryExistsAsync(
                request.CategoryId))
            {
                throw new InvalidOperationException(
                    "Kategori yang dipilih tidak ditemukan.");
            }

            if (!await _gearItemRepository.BrandExistsAsync(
                request.BrandId))
            {
                throw new InvalidOperationException(
                    "Brand yang dipilih tidak ditemukan.");
            }
        }

        private static void ValidateBusinessRules(
            GearItemRequest request)
        {
            if (request.DailyPrice <= 0)
            {
                throw new InvalidOperationException(
                    "Harga sewa per hari harus lebih besar dari nol.");
            }

            if (request.ReplacementPrice <= 0)
            {
                throw new InvalidOperationException(
                    "Harga penggantian harus lebih besar dari nol.");
            }

            if (request.TotalStock < 0 ||
                request.MaintenanceStock < 0 ||
                request.DamagedStock < 0)
            {
                throw new InvalidOperationException(
                    "Jumlah stok tidak boleh bernilai negatif.");
            }
        }

        private static GearItemResponse MapToResponse(
            GearItem gearItem)
        {
            return new GearItemResponse
            {
                GearItemId = gearItem.GearItemId,
                CategoryId = gearItem.CategoryId,
                CategoryName = gearItem.Category?.CategoryName
                    ?? string.Empty,
                BrandId = gearItem.BrandId,
                BrandName = gearItem.Brand?.BrandName
                    ?? string.Empty,
                GearName = gearItem.GearName,
                Description = gearItem.Description,
                DailyPrice = gearItem.DailyPrice,
                ReplacementPrice = gearItem.ReplacementPrice,
                TotalStock = gearItem.TotalStock,
                MaintenanceStock = gearItem.MaintenanceStock,
                DamagedStock = gearItem.DamagedStock,
                IsActive = gearItem.IsActive,
                ImageUrl = gearItem.ImageUrl
            };
        }
    }
}
