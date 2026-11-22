using HikeRent.Exceptions;
using HikeRent.DTOs.Payments;
using HikeRent.Enums;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HikeRent.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PaymentService> _logger;

        private static readonly TimeZoneInfo JakartaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

        public PaymentService(
            IPaymentRepository paymentRepository,
            IRentalRepository rentalRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _rentalRepository = rentalRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IEnumerable<PaymentResponse>> GetAllAsync()
        {
            IEnumerable<Payment> payments = await _paymentRepository
                .GetAllWithDetailsAsync();

            return payments.Select(MapToResponse).ToList();
        }

        public async Task<PaymentResponse?> GetByRentalIdAsync(int rentalId)
        {
            Payment? payment = await _paymentRepository
                .GetByRentalIdWithDetailsAsync(rentalId);

            if (payment == null)
            {
                return null;
            }

            return MapToResponse(payment);
        }

        public async Task<PaymentResponse?> CreateAsync(
            int rentalId,
            PaymentRequest request)
        {
            string? userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new BusinessException("User ID tidak valid.");
            }

            if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
            {
                throw new BusinessException(
                    "Metode pembayaran tidak valid.");
            }

            bool transactionStarted = false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                Rental? rental = await _rentalRepository
                    .GetByIdWithDetailsForUpdateAsync(rentalId);

                if (rental == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    transactionStarted = false;

                    return null;
                }

                if (rental.Status != RentalStatus.Returned)
                {
                    throw new BusinessException(
                        "Pembayaran hanya dapat dilakukan untuk Rental " +
                        "dengan status Returned.");
                }

                if (rental.PaymentStatus == PaymentStatus.Paid)
                {
                    throw new BusinessException(
                        "Rental ini sudah dibayar.");
                }

                Payment? existingPayment = await _paymentRepository
                    .GetByRentalIdWithDetailsAsync(rentalId);

                if (existingPayment != null)
                {
                    throw new BusinessException(
                        "Rental ini sudah memiliki data pembayaran.");
                }

                if (rental.GrandTotal <= 0)
                {
                    throw new BusinessException(
                        "Grand Total Rental tidak valid untuk dibayar.");
                }

                DateTime paymentDateUtc = DateTime.UtcNow;

                DateTime paymentLocalTime = TimeZoneInfo.ConvertTimeFromUtc(
                    paymentDateUtc,
                    JakartaTimeZone);

                Payment payment = new Payment
                {
                    RentalId = rental.RentalId,
                    UserId = userId,

                    PaymentCode =
                        $"PAY-{paymentLocalTime:yyyyMMdd}-{Guid.NewGuid():N}",

                    Amount = rental.GrandTotal,
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = paymentDateUtc
                };

                await _paymentRepository.AddAsync(payment);

                // PaymentStatus disimpan di Rental dan harus konsisten
                // dengan keberadaan data Payment.
                rental.PaymentStatus = PaymentStatus.Paid;
                rental.Status = RentalStatus.Completed;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                _logger.LogInformation("Pembayaran {PaymentCode} untuk Rental {RentalId} berhasil", payment.PaymentCode, rental.RentalId);

                Payment? savedPayment = await _paymentRepository
                    .GetByRentalIdWithDetailsAsync(rentalId);

                if (savedPayment == null)
                {
                    throw new BusinessException(
                        "Data pembayaran gagal disimpan.");
                }

                return MapToResponse(savedPayment);
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

        private static PaymentResponse MapToResponse(Payment payment)
        {
            return new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                PaymentCode = payment.PaymentCode,
                RentalId = payment.RentalId,
                RentalCode = payment.Rental?.RentalCode ?? string.Empty,
                CustomerName =
                    payment.Rental?.Customer?.FullName ?? string.Empty,
                Amount = payment.Amount,
                GrandTotal = payment.Rental?.GrandTotal ?? 0,
                PaymentMethod = payment.PaymentMethod.ToString(),
                PaymentDate = payment.PaymentDate,
                PaymentStatus =
                    payment.Rental?.PaymentStatus.ToString() ?? string.Empty
            };
        }
    }
}
