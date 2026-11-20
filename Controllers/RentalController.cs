using HikeRent.DTOs.Rentals;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/rental")]
    [Authorize]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        // POST: api/rental
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] RentalRequest request)
        {
            var result = await _rentalService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.RentalId },
                new
                {
                    status = "success",
                    message = "Rental berhasil dibuat.",
                    data = result
                });
        }

        // GET: api/rental/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalService.GetByIdAsync(id);

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
                message = "Data Rental berhasil ditemukan.",
                data = result
            });
        }

        // GET: api/rental
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalService.GetAllAsync();

            return Ok(new
            {
                status = "success",
                message = "Data Rental berhasil diambil.",
                data = result
            });
        }
    }
}
