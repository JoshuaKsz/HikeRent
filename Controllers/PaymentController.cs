using HikeRent.DTOs.Payments;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRentalService _rentalService;

        public PaymentController(
            IPaymentService paymentService,
            IRentalService rentalService)
        {
            _paymentService = paymentService;
            _rentalService = rentalService;
        }

        // GET: api/payment/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _rentalService.GetReturnedUnpaidAsync();

            return Ok(new
            {
                status = "success",
                message = "Data Rental yang menunggu pembayaran berhasil diambil.",
                data = result
            });
        }

        // GET: api/payment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllAsync();

            return Ok(new
            {
                status = "success",
                message = "Data pembayaran berhasil diambil.",
                data = result
            });
        }

        // GET: api/payment/{rentalId}
        [HttpGet("{rentalId:int}")]
        public async Task<IActionResult> GetByRentalId(int rentalId)
        {
            var result = await _paymentService.GetByRentalIdAsync(rentalId);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Data pembayaran tidak ditemukan."
                });
            }

            return Ok(new
            {
                status = "success",
                message = "Data pembayaran berhasil ditemukan.",
                data = result
            });
        }

        // POST: api/payment/{rentalId}
        [HttpPost("{rentalId:int}")]
        public async Task<IActionResult> Create(
            int rentalId,
            [FromBody] PaymentRequest request)
        {
            var result = await _paymentService.CreateAsync(rentalId, request);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Rental tidak ditemukan."
                });
            }

            return Ok(new
            {
                status = "success",
                message = "Pembayaran berhasil dicatat.",
                data = result
            });
        }
    }
}
