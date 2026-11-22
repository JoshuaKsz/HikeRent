using HikeRent.Exceptions;
﻿using HikeRent.DTOs.Rentals;
using HikeRent.Enums;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HikeRent.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IGearItemRepository _gearItemRepository;
        private readonly IPricingService _pricingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RentalService> _logger;
        private readonly IActivityLogService _activityLogService;

        private static readonly TimeZoneInfo JakartaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

        public RentalService(
            IRentalRepository rentalRepository,
            ICustomerRepository customerRepository,
            IGearItemRepository gearItemRepository,
            IPricingService pricingService,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RentalService> logger,
            IActivityLogService activityLogService)
        {
            _rentalRepository = rentalRepository;
            _customerRepository = customerRepository;
            _gearItemRepository = gearItemRepository;
            _pricingService = pricingService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        public async Task<RentalResponse> CreateAsync(RentalRequest request)
        {
            // Mendapatkan UserId dari JWT
            string? userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new BusinessException("User ID tidak valid.");
            }

            // Validasi Customer
            Customer? customer = await _customerRepository
                .GetByIdAsync(request.CustomerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer tidak ditemukan.");
            }

            // Validasi request
            if (request.Details == null || request.Details.Count == 0)
            {
                throw new BusinessException(
                    "Minimal harus ada satu peralatan yang disewa.");
            }

            if (request.Details.Any(d =>
                d.GearItemId <= 0 || d.Quantity <= 0))
            {
                throw new BusinessException(
                    "GearItemId dan Quantity harus lebih besar dari nol.");
            }

            if (request.Details
                .GroupBy(d => d.GearItemId)
                .Any(g => g.Count() > 1))
            {
                throw new BusinessException(
                    "Peralatan yang sama tidak boleh dimasukkan lebih dari satu kali.");
            }

            DateTime rentalDateUtc = DateTime.UtcNow;

            DateTime rentalLocalTime = TimeZoneInfo.ConvertTimeFromUtc(
                rentalDateUtc,
                JakartaTimeZone);

            // Hitung durasi dan validasi tanggal via PricingService
            int rentalDays = _pricingService.CalculateRentalDays(
                rentalLocalTime,
                request.PlannedStartDate,
                request.PlannedReturnDate
            );

            bool transactionStarted = false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                List<RentalDetail> rentalDetails = new List<RentalDetail>();

                decimal totalRentalFee = 0;

                Dictionary<int, string> gearItemNames = new Dictionary<int, string>();

                // Mengunci GearItem berdasarkan urutan ID
                List<RentalDetailRequest> sortedDetails = request.Details
                    .OrderBy(d => d.GearItemId)
                    .ToList();

                foreach (RentalDetailRequest detail in sortedDetails)
                {
                    GearItem? gearItem = await _gearItemRepository
                        .GetForStockUpdateAsync(detail.GearItemId);

                    if (gearItem == null)
                    {
                        throw new KeyNotFoundException(
                            $"GearItem dengan ID {detail.GearItemId} tidak ditemukan.");
                    }

                    if (!gearItem.IsActive)
                    {
                        throw new BusinessException(
                            $"GearItem {gearItem.GearName} tidak aktif.");
                    }

                    int rentedStock = await _gearItemRepository
                        .GetRentedStockAsync(gearItem.GearItemId);

                    int availableStock =
                        gearItem.TotalStock -
                        gearItem.DamagedStock -
                        rentedStock;

                    if (availableStock < detail.Quantity)
                    {
                        throw new BusinessException(
                            $"Stok {gearItem.GearName} tidak mencukupi. " +
                            $"Stok tersedia: {availableStock}.");
                    }

                    // Hitung subtotal via PricingService dengan aturan bertingkat
                    decimal subtotal = _pricingService.CalculateSubtotal(
                        detail.Quantity,
                        gearItem.DailyPrice,
                        rentalDays
                    );

                    totalRentalFee += subtotal;

                    // Simpan nama GearItem berdasarkan ID
                    gearItemNames.Add(
                        gearItem.GearItemId,
                        gearItem.GearName
                    );

                    rentalDetails.Add(new RentalDetail
                    {
                        GearItemId = gearItem.GearItemId,
                        Quantity = detail.Quantity,
                        DailyPrice = gearItem.DailyPrice,
                        ReplacementPrice = gearItem.ReplacementPrice,
                        RentalDays = rentalDays,
                        Subtotal = subtotal
                    });
                }

                Rental rental = new Rental
                {
                    CustomerId = customer.CustomerId,
                    UserId = userId,

                    RentalCode =
                        $"RNT-{rentalLocalTime:yyyyMMdd}-{Guid.NewGuid():N}",

                    RentalDate = rentalDateUtc,
                    PlannedStartDate = request.PlannedStartDate,
                    PlannedReturnDate = request.PlannedReturnDate,

                    Status = RentalStatus.Ongoing,
                    PaymentStatus = PaymentStatus.Unpaid,

                    TotalRentalFee = totalRentalFee,
                    TotalLateFee = 0,
                    TotalDamageFee = 0,
                    TotalLostItemFee = 0,
                    GrandTotal = totalRentalFee,

                    CreatedAt = rentalDateUtc,

                    RentalDetails = rentalDetails
                };

                await _rentalRepository.AddAsync(rental);

                await _activityLogService.CreateAsync(
                    new HikeRent.DTOs.ActivityLog.ActivityLogRequest
                    {
                        Activity = $"Membuat rental {rental.RentalCode} untuk customer {customer.FullName}"
                    },
                    userId);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                _logger.LogInformation("Rental {RentalCode} dibuat untuk Customer {CustomerId}", rental.RentalCode, rental.CustomerId);

                return new RentalResponse
                {
                    RentalId = rental.RentalId,
                    CustomerId = rental.CustomerId,
                    CustomerName = customer.FullName,
                    RentalCode = rental.RentalCode,
                    RentalDate = rental.RentalDate,
                    PlannedStartDate = rental.PlannedStartDate,
                    PlannedReturnDate = rental.PlannedReturnDate,
                    Status = rental.Status.ToString(),

                    TotalRentalFee = rental.TotalRentalFee,
                    TotalLateFee = rental.TotalLateFee,
                    TotalDamageFee = rental.TotalDamageFee,
                    TotalLostItemFee = rental.TotalLostItemFee,
                    GrandTotal = rental.GrandTotal,

                    PaymentStatus = rental.PaymentStatus.ToString(),
                    CreatedAt = rental.CreatedAt,

                    Details = rental.RentalDetails.Select(rd =>
                        new RentalDetailResponse
                        {
                            RentalDetailId = rd.RentalDetailId,
                            GearItemId = rd.GearItemId,

                            GearName = gearItemNames[rd.GearItemId],

                            Quantity = rd.Quantity,
                            DailyPrice = rd.DailyPrice,
                            ReplacementPrice = rd.ReplacementPrice,
                            RentalDays = rd.RentalDays,
                            Subtotal = rd.Subtotal
                        }).ToList()
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
        public async Task<IEnumerable<RentalResponse>> GetAllAsync()
        {
            IEnumerable<Rental> rentals = await _rentalRepository.GetAllWithDetailsAsync();

            return rentals.Select(MapToResponse).ToList();
        }

        public async Task<RentalResponse?> GetByIdAsync(int id)
        {
            Rental? rental = await _rentalRepository.GetByIdWithDetailsAsync(id);

            if (rental == null)
            {
                return null;
            }

            return MapToResponse(rental);
        }

        public async Task<IEnumerable<RentalResponse>> GetReturnedUnpaidAsync()
        {
            IEnumerable<Rental> rentals = await _rentalRepository
                .GetReturnedUnpaidAsync();

            return rentals.Select(MapToResponse).ToList();
        }

        private static RentalResponse MapToResponse(Rental rental)
        {
            return new RentalResponse
            {
                RentalId = rental.RentalId,
                CustomerId = rental.CustomerId,
                CustomerName = rental.Customer?.FullName ?? string.Empty,
                RentalCode = rental.RentalCode,
                RentalDate = rental.RentalDate,
                PlannedStartDate = rental.PlannedStartDate,
                PlannedReturnDate = rental.PlannedReturnDate,
                Status = rental.Status.ToString(),

                TotalRentalFee = rental.TotalRentalFee,
                TotalLateFee = rental.TotalLateFee,
                TotalDamageFee = rental.TotalDamageFee,
                TotalLostItemFee = rental.TotalLostItemFee,
                GrandTotal = rental.GrandTotal,

                PaymentStatus = rental.PaymentStatus.ToString(),
                CreatedAt = rental.CreatedAt,

                Details = rental.RentalDetails.Select(rd =>
                    new RentalDetailResponse
                    {
                        RentalDetailId = rd.RentalDetailId,
                        GearItemId = rd.GearItemId,
                        GearName = rd.GearItem?.GearName ?? string.Empty,
                        Quantity = rd.Quantity,
                        DailyPrice = rd.DailyPrice,
                        ReplacementPrice = rd.ReplacementPrice,
                        RentalDays = rd.RentalDays,
                        Subtotal = rd.Subtotal
                    }).ToList()
            };
        }
    }
}
