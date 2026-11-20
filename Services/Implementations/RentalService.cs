using HikeRent.DTOs.Rentals;
using HikeRent.Enums;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using System.Security.Claims;

namespace HikeRent.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IGearItemRepository _gearItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly TimeZoneInfo JakartaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

        public RentalService(
            IRentalRepository rentalRepository,
            ICustomerRepository customerRepository,
            IGearItemRepository gearItemRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _rentalRepository = rentalRepository;
            _customerRepository = customerRepository;
            _gearItemRepository = gearItemRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RentalResponse> CreateAsync(RentalRequest request)
        {
            // Mendapatkan UserId dari JWT
            string? userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new InvalidOperationException("User ID tidak valid.");
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
                throw new InvalidOperationException(
                    "Minimal harus ada satu peralatan yang disewa.");
            }

            if (request.Details.Any(d =>
                d.GearItemId <= 0 || d.Quantity <= 0))
            {
                throw new InvalidOperationException(
                    "GearItemId dan Quantity harus lebih besar dari nol.");
            }

            if (request.Details
                .GroupBy(d => d.GearItemId)
                .Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException(
                    "Peralatan yang sama tidak boleh dimasukkan lebih dari satu kali.");
            }

            DateTime rentalDateUtc = DateTime.UtcNow;

            DateTime rentalLocalTime = TimeZoneInfo.ConvertTimeFromUtc(
                rentalDateUtc,
                JakartaTimeZone);

            // Menghitung tanggal mulai efektif
            DateOnly effectiveStartDate =
                rentalLocalTime.TimeOfDay >= new TimeSpan(12, 0, 0)
                    ? DateOnly.FromDateTime(rentalLocalTime).AddDays(1)
                    : DateOnly.FromDateTime(rentalLocalTime);

            // Validasi tanggal Rental
            if (request.PlannedReturnDate < request.PlannedStartDate)
            {
                throw new InvalidOperationException(
                    "Tanggal pengembalian tidak boleh sebelum tanggal mulai sewa.");
            }

            if (request.PlannedStartDate != effectiveStartDate)
            {
                throw new InvalidOperationException(
                    $"Tanggal mulai sewa harus {effectiveStartDate:dd-MM-yyyy} " +
                    "sesuai waktu transaksi dan aturan cut-off pukul 12.00 WIB.");
            }

            int rentalDays = Math.Max(
                1,
                request.PlannedReturnDate.DayNumber -
                effectiveStartDate.DayNumber + 1
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
                        throw new InvalidOperationException(
                            $"GearItem {gearItem.GearName} tidak aktif.");
                    }

                    int rentedStock = await _gearItemRepository
                        .GetRentedStockAsync(gearItem.GearItemId);

                    int availableStock =
                        gearItem.TotalStock -
                        gearItem.MaintenanceStock -
                        gearItem.DamagedStock -
                        rentedStock;

                    if (availableStock < detail.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Stok {gearItem.GearName} tidak mencukupi. " +
                            $"Stok tersedia: {availableStock}.");
                    }

                    decimal subtotal =
                        detail.Quantity *
                        gearItem.DailyPrice *
                        rentalDays;

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

                    Status = RentalStatus.Pending,
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

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

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

            return rentals.Select(r => new RentalResponse
            {
                RentalId = r.RentalId,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer?.FullName ?? string.Empty,
                RentalCode = r.RentalCode,
                RentalDate = r.RentalDate,
                PlannedStartDate = r.PlannedStartDate,
                PlannedReturnDate = r.PlannedReturnDate,
                Status = r.Status.ToString(),

                TotalRentalFee = r.TotalRentalFee,
                TotalLateFee = r.TotalLateFee,
                TotalDamageFee = r.TotalDamageFee,
                TotalLostItemFee = r.TotalLostItemFee,
                GrandTotal = r.GrandTotal,

                PaymentStatus = r.PaymentStatus.ToString(),
                CreatedAt = r.CreatedAt,

                Details = r.RentalDetails.Select(rd =>
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
            }).ToList();
        }

        public async Task<RentalResponse?> GetByIdAsync(int id)
        {
            Rental? rental = await _rentalRepository.GetByIdWithDetailsAsync(id);

            if (rental == null)
            {
                return null;
            }

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

        public async Task<RentalResponse?> UpdateStatusAsync(int id, string status)
        {
            // Validasi status yang dikirim
            bool isValidStatus = Enum.TryParse(
                status,
                true,
                out RentalStatus newStatus
            );

            if (!isValidStatus ||
                !Enum.IsDefined(typeof(RentalStatus), newStatus) ||
                !string.Equals(
                    status?.Trim(),
                    newStatus.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Status Rental tidak valid."
                );
            }

            bool transactionStarted = false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                // Mengambil Rental dengan tracking
                Rental? rental = await _rentalRepository
                    .GetByIdWithDetailsForUpdateAsync(id);

                if (rental == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    transactionStarted = false;

                    return null;
                }

                // Validasi transisi status
                if (rental.Status == RentalStatus.Pending &&
                    newStatus == RentalStatus.Ongoing)
                {
                    rental.Status = RentalStatus.Ongoing;
                }
                else if (rental.Status == RentalStatus.Ongoing &&
                         newStatus == RentalStatus.Completed)
                {
                    // Memastikan Rental memiliki detail
                    if (rental.RentalDetails.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Rental tidak memiliki detail peralatan."
                        );
                    }

                    // Memastikan seluruh peralatan sudah dikembalikan
                    bool allReturned = rental.RentalDetails.All(rd =>
                        rd.ReturnRecord != null &&
                        rd.ReturnRecord.QuantityReturned +
                        rd.ReturnRecord.LostQuantity == rd.Quantity &&
                        rd.ReturnRecord.DamagedQuantity <=
                        rd.ReturnRecord.QuantityReturned
                    );

                    if (!allReturned)
                    {
                        throw new InvalidOperationException(
                            "Rental tidak dapat diselesaikan karena masih ada peralatan yang belum dikembalikan sepenuhnya."
                        );
                    }

                    rental.Status = RentalStatus.Completed;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Perubahan status dari {rental.Status} " +
                        $"menjadi {newStatus} tidak diizinkan."
                    );
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                // Mengambil kembali Rental untuk response
                return await GetByIdAsync(id);
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
        public async Task<bool> CancelAsync(int id)
        {
            bool transactionStarted = false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                // Mengambil Rental dengan tracking
                Rental? rental = await _rentalRepository
                    .GetByIdWithDetailsForUpdateAsync(id);

                if (rental == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    transactionStarted = false;

                    return false;
                }

                // Hanya Rental Pending yang boleh dibatalkan
                if (rental.Status != RentalStatus.Pending)
                {
                    throw new InvalidOperationException(
                        "Hanya Rental dengan status Pending yang dapat dibatalkan."
                    );
                }

                // Mengubah status Rental
                rental.Status = RentalStatus.Cancelled;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                return true;
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
    }
}
