using HikeRent.Exceptions;
﻿using HikeRent.DTOs.Customer;
using HikeRent.DTOs.Rentals;
using HikeRent.DTOs.ReturnRecord;
using HikeRent.Models;
using HikeRent.Repositories.Implementations;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HikeRent.Services.Implementations
{
    public class ReturnRecordService : IReturnRecordService
    {
        private readonly IReturnRecordRepository _returnRecordRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IGearItemRepository _gearItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReturnRecordService> _logger;
        private readonly IActivityLogService _activityLogService;

        private static readonly TimeZoneInfo JakartaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

        public ReturnRecordService(
            IReturnRecordRepository returnRecordRepository,
            IRentalRepository rentalRepository,
            IGearItemRepository gearItemRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReturnRecordService> logger,
            IActivityLogService activityLogService)
        {
            _returnRecordRepository = returnRecordRepository;
            _rentalRepository = rentalRepository;
            _gearItemRepository = gearItemRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        public async Task<ReturnRecordResponse> CreateAsync(
            ReturnRecordRequest request)
        {
            // Mendapatkan UserId dari JWT
            string? userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new BusinessException(
                    "User ID tidak valid.");
            }

            // Validasi RentalDetail
            if (request.RentalDetailId <= 0)
            {
                throw new BusinessException(
                    "RentalDetailId tidak valid.");
            }

            if (request.QuantityReturned < 0 ||
                request.DamagedQuantity < 0 ||
                request.LostQuantity < 0)
            {
                throw new BusinessException(
                    "Jumlah barang tidak boleh negatif.");
            }

            if (request.QuantityReturned == 0 &&
                request.DamagedQuantity == 0 &&
                request.LostQuantity == 0)
            {
                throw new BusinessException(
                    "Minimal harus ada satu barang yang dikembalikan, rusak, atau hilang.");
            }

            DateTime returnDateUtc = request.ActualReturnDate != default
                ? request.ActualReturnDate.ToUniversalTime()
                : DateTime.UtcNow;

            DateTime returnLocalTime = TimeZoneInfo.ConvertTimeFromUtc(
                returnDateUtc,
                JakartaTimeZone);

            bool transactionStarted = false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                // Mengambil Rental beserta seluruh RentalDetail
                Rental? rental = await _rentalRepository
                    .GetByIdWithDetailsForUpdateAsync(request.RentalId);

                if (rental == null)
                {
                    throw new KeyNotFoundException(
                        "Rental tidak ditemukan.");
                }

                RentalDetail? rentalDetail = rental.RentalDetails
                    .FirstOrDefault(rd =>
                        rd.RentalDetailId == request.RentalDetailId);

                if (rentalDetail == null)
                {
                    throw new KeyNotFoundException(
                        "RentalDetail tidak ditemukan.");
                }

                // Validasi status rental
                if (rental.Status != Enums.RentalStatus.Ongoing)
                {
                    throw new BusinessException(
                        "Rental tidak dalam status ongoing.");
                }

                // Validasi apakah detail sudah dikembalikan
                if (rentalDetail.ReturnRecord != null)
                {
                    throw new BusinessException(
                        "Peralatan ini sudah memiliki data pengembalian.");
                }

                // Validasi jumlah barang
                int totalProcessed =
                    request.QuantityReturned +
                    request.DamagedQuantity +
                    request.LostQuantity;

                if (totalProcessed != rentalDetail.Quantity)
                {
                    throw new BusinessException(
                        $"Total barang yang diproses harus sama dengan jumlah sewa ({rentalDetail.Quantity}).");
                }

                // Mengunci GearItem untuk update stok
                GearItem? gearItem = await _gearItemRepository
                    .GetForStockUpdateAsync(rentalDetail.GearItemId);

                if (gearItem == null)
                {
                    throw new KeyNotFoundException(
                        "GearItem tidak ditemukan.");
                }

                // MENGHITUNG DENDA KETERLAMBATAN
                // Hitung jumlah hari keterlambatan
                DateOnly actualReturnDate = DateOnly.FromDateTime(returnLocalTime);

                int lateDays = Math.Max(
                    0,
                    actualReturnDate.DayNumber -
                    rental.PlannedReturnDate.DayNumber
                );

                // Persentase progresif setiap hari keterlambatan
                const decimal progressiveRatePerDay = 0.10m;

                // Total biaya sewa harian untuk seluruh barang
                decimal dailyRentalTotal =
                    rentalDetail.DailyPrice * rentalDetail.Quantity;

                decimal lateFee = 0;

                // Contoh:
                // Hari 1 = 10%
                // Hari 2 = 20%
                // Hari 3 = 30%
                // Hari 4 = 40%
                for (int day = 1; day <= lateDays; day++)
                {
                    decimal dailyProgressiveRate =
                        progressiveRatePerDay * day;

                    lateFee +=
                        dailyRentalTotal * dailyProgressiveRate;
                }

                lateFee = decimal.Round(
                    lateFee,
                    2,
                    MidpointRounding.AwayFromZero
                );

                // MENGHITUNG DENDA KERUSAKAN
                decimal damageFee =
                    request.DamagedQuantity *
                    rentalDetail.ReplacementPrice;

                damageFee = decimal.Round(
                    damageFee,
                    2,
                    MidpointRounding.AwayFromZero
                );

                // MENGHITUNG DENDA KEHILANGAN
                decimal lostItemFee =
                    request.LostQuantity *
                    rentalDetail.ReplacementPrice;

                lostItemFee = decimal.Round(
                    lostItemFee,
                    2,
                    MidpointRounding.AwayFromZero
                );

                // MEMBUAT RETURN RECORD
                ReturnRecord returnRecord = new ReturnRecord
                {
                    RentalDetailId = rentalDetail.RentalDetailId,
                    UserId = userId,

                    QuantityReturned = request.QuantityReturned,
                    DamagedQuantity = request.DamagedQuantity,
                    LostQuantity = request.LostQuantity,

                    Notes = request.Notes,

                    LateFee = lateFee,
                    DamageFee = damageFee,
                    LostItemFee = lostItemFee,

                    ActualReturnDate = returnDateUtc
                };

                await _returnRecordRepository.AddAsync(returnRecord);

                // Hubungkan navigation property agar status
                // seluruh RentalDetail bisa diperiksa sebelum SaveChanges.
                rentalDetail.ReturnRecord = returnRecord;

                // UPDATE STOK GEAR ITEM
                gearItem.DamagedStock += request.DamagedQuantity;
                gearItem.TotalStock -= request.LostQuantity;

                // AGREGASI FEE KE RENTAL HEADER
                rental.TotalLateFee += lateFee;
                rental.TotalDamageFee += damageFee;
                rental.TotalLostItemFee += lostItemFee;

                rental.GrandTotal =
                    rental.TotalRentalFee +
                    rental.TotalLateFee +
                    rental.TotalDamageFee +
                    rental.TotalLostItemFee;

                // PERIKSA STATUS SELURUH RENTAL DETAIL
                bool allReturned = rental.RentalDetails
                    .All(rd => rd.ReturnRecord != null);

                if (allReturned)
                {
                    rental.Status = Enums.RentalStatus.Returned;
                }

                await _activityLogService.CreateAsync(
                    new HikeRent.DTOs.ActivityLog.ActivityLogRequest
                    {
                        Activity = $"Memproses pengembalian rental {rental.RentalCode} (Detail {rentalDetail.RentalDetailId})"
                    },
                    userId);

                // SIMPAN SEMUA PERUBAHAN SEKALI SAJA
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                _logger.LogInformation("ReturnRecord dibuat untuk RentalDetailId {DetailId} oleh User {UserId}", rentalDetail.RentalDetailId, userId);

                // RETURN RESPONSE
                return new ReturnRecordResponse
                {
                    ReturnRecordId = returnRecord.ReturnRecordId,

                    UserId = returnRecord.UserId,

                    QuantityReturned = returnRecord.QuantityReturned,
                    DamagedQuantity = returnRecord.DamagedQuantity,
                    LostQuantity = returnRecord.LostQuantity,

                    Notes = returnRecord.Notes,

                    LateFee = returnRecord.LateFee,
                    DamageFee = returnRecord.DamageFee,
                    LostItemFee = returnRecord.LostItemFee,

                    ActualReturnDate = returnRecord.ActualReturnDate
                };
            }
            catch
            {
                if (transactionStarted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }

                throw;
            }
        }
        public async Task<IEnumerable<ReturnRecordResponse>> GetAllAsync()
        {
            IEnumerable<ReturnRecord> returnRecords = await this._returnRecordRepository.GetAllAsync();

            return returnRecords.Select(returnRecord => new ReturnRecordResponse
            {
                ReturnRecordId = returnRecord.ReturnRecordId,
                //RentalDetailId = returnRecord.RentalDetailId,

                UserId = returnRecord.UserId,


                QuantityReturned = returnRecord.QuantityReturned,
                DamagedQuantity = returnRecord.DamagedQuantity,
                LostQuantity = returnRecord.LostQuantity,

                Notes = returnRecord.Notes,

                LateFee = returnRecord.LateFee,
                DamageFee = returnRecord.DamageFee,
                LostItemFee = returnRecord.LostItemFee,

                ActualReturnDate = returnRecord.ActualReturnDate
            });
        }
        public async Task<ReturnRecordResponse?> GetByRentalDetailIdAsync(int rentalDetailId)
        {

            ReturnRecord? returnRecord = await this._returnRecordRepository.GetByRentalDetailIdAsync(rentalDetailId);

            if (returnRecord == null)
            {
                return null;
            }
            return new ReturnRecordResponse
            {
                ReturnRecordId = returnRecord.ReturnRecordId,
                //RentalDetailId = returnRecord.RentalDetailId,

                UserId = returnRecord.UserId,


                QuantityReturned = returnRecord.QuantityReturned,
                DamagedQuantity = returnRecord.DamagedQuantity,
                LostQuantity = returnRecord.LostQuantity,

                Notes = returnRecord.Notes,

                LateFee = returnRecord.LateFee,
                DamageFee = returnRecord.DamageFee,
                LostItemFee = returnRecord.LostItemFee,

                ActualReturnDate = returnRecord.ActualReturnDate
            };



        }

        public async Task<ReturnRecordResponse?> GetByIdAsync(int id)
        {
            ReturnRecord? returnRecord = await _returnRecordRepository.GetByIdAsync(id);

            if (returnRecord == null)
            {
                return null;
            }

            return new ReturnRecordResponse
            {
                ReturnRecordId = returnRecord.ReturnRecordId,
                //RentalDetailId = returnRecord.RentalDetailId,

                UserId = returnRecord.UserId,


                QuantityReturned = returnRecord.QuantityReturned,
                DamagedQuantity = returnRecord.DamagedQuantity,
                LostQuantity = returnRecord.LostQuantity,

                Notes = returnRecord.Notes,

                LateFee = returnRecord.LateFee,
                DamageFee = returnRecord.DamageFee,
                LostItemFee = returnRecord.LostItemFee,

                ActualReturnDate = returnRecord.ActualReturnDate
            };
        }
    }
}
