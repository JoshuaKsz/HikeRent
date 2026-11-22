using HikeRent.Exceptions;
﻿using HikeRent.DTOs.GearItems;
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

            var rentedStocks =
                await _gearItemRepository.GetRentedStockByGearItemAsync(
                    gearItems.Select(g => g.GearItemId)
                );

            return gearItems.Select(gearItem =>
            {
                rentedStocks.TryGetValue(
                    gearItem.GearItemId,
                    out int rentedStock
                );

                return MapToResponse(gearItem, rentedStock);
            });
        }

        public async Task<GearItemResponse?> GetByIdAsync(int id)
        {
            GearItem? gearItem =
                await _gearItemRepository.GetByIdWithDetailsAsync(id);

            if (gearItem == null)
            {
                return null;
            }

            int rentedStock =
                await _gearItemRepository.GetRentedStockAsync(id);

            return MapToResponse(gearItem, rentedStock);
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
                DamagedStock = request.DamagedStock,
                ImageUrl = request.ImageUrl?.Trim(),
                IsActive = request.IsActive
            };

            await _gearItemRepository.AddAsync(gearItem);

            await _unitOfWork.SaveChangesAsync();

            GearItem? savedGearItem =
                await _gearItemRepository.GetByIdWithDetailsAsync(gearItem.GearItemId);

            int rentedStock =
                await _gearItemRepository.GetRentedStockAsync(gearItem.GearItemId);

            return MapToResponse(savedGearItem!, rentedStock);
        }


        public async Task<GearItemResponse?> UpdateAsync(
            int id,
            GearItemRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Lock GearItem row until transaction completes.
                GearItem? gearItem =
                    await _gearItemRepository.GetForStockUpdateAsync(id);

                if (gearItem == null)
                {
                    await _unitOfWork.CommitTransactionAsync();
                    return null;
                }

                await ValidateReferencesAsync(request);

                ValidateBusinessRules(request);

                // Calculate rented stock using the latest committed data.
                int rentedStock =
                    await _gearItemRepository.GetRentedStockAsync(id);

                int availableStock =
                    request.TotalStock
                    - request.DamagedStock
                    - rentedStock;

                if (availableStock < 0)
                {
                    throw new BusinessException(
                        "Perubahan stok ditolak karena jumlah stok tersedia tidak boleh negatif.");
                }

                if (gearItem.IsActive && !request.IsActive)
                {
                    bool hasActiveRental =
                        await _gearItemRepository.HasActiveRentalAsync(id);

                    if (hasActiveRental)
                    {
                        throw new BusinessException(
                            "GearItem tidak dapat dinonaktifkan karena masih memiliki Rental Pending atau Ongoing."
                        );
                    }
                }

                // Update GearItem properties.
                gearItem.CategoryId = request.CategoryId;
                gearItem.BrandId = request.BrandId;
                gearItem.GearName = request.GearName.Trim();
                gearItem.Description = request.Description?.Trim();
                gearItem.DailyPrice = request.DailyPrice;
                gearItem.ReplacementPrice = request.ReplacementPrice;
                gearItem.TotalStock = request.TotalStock;
                gearItem.DamagedStock = request.DamagedStock;
                gearItem.ImageUrl = request.ImageUrl?.Trim();
                gearItem.IsActive = request.IsActive;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                GearItem? updatedGearItem =
                    await _gearItemRepository.GetByIdWithDetailsAsync(id);

                return MapToResponse(updatedGearItem!, rentedStock);
            }
            catch
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch
                {

                }

                throw;
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            GearItem? gearItem =
                await _gearItemRepository.GetByIdAsync(id);

            if (gearItem == null)
            {
                return false;
            }

            bool hasRentalHistory =
                await _gearItemRepository.HasRentalHistoryAsync(id);

            if (hasRentalHistory)
            {
                throw new BusinessException(
                    "GearItem tidak dapat dihapus karena sudah memiliki riwayat Rental."
                );
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
                throw new BusinessException(
                    "Kategori yang dipilih tidak ditemukan.");
            }

            if (!await _gearItemRepository.BrandExistsAsync(
                request.BrandId))
            {
                throw new BusinessException(
                    "Brand yang dipilih tidak ditemukan.");
            }
        }

        private static void ValidateBusinessRules(
            GearItemRequest request)
        {
            if (request.DailyPrice <= 0)
            {
                throw new BusinessException(
                    "Harga sewa per hari harus lebih besar dari nol.");
            }

            if (request.ReplacementPrice <= 0)
            {
                throw new BusinessException(
                    "Harga penggantian harus lebih besar dari nol.");
            }

            if (request.TotalStock < 0 ||
                request.DamagedStock < 0)
            {
                throw new BusinessException(
                    "Jumlah stok tidak boleh bernilai negatif.");
            }

            if (request.DamagedStock > request.TotalStock)
            {
                throw new BusinessException(
                    "Jumlah stok rusak tidak boleh melebihi total stok.");
            }
        }

        private static GearItemResponse MapToResponse(
            GearItem gearItem,
            int rentedStock)
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
                DamagedStock = gearItem.DamagedStock,
                AvailableStock = CalculateAvailableStock(gearItem, rentedStock),
                IsActive = gearItem.IsActive,
                ImageUrl = gearItem.ImageUrl
            };
        }
        private static int CalculateAvailableStock(GearItem gearItem, int rentedStock)
        {
            return gearItem.TotalStock
                - gearItem.DamagedStock
                - rentedStock;
        }
    }
}
