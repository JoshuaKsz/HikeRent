using HikeRent.DTOs.Brands;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/brand")]
    [Authorize]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<BrandResponse> brands = await _brandService.GetAllAsync();

            return Ok(brands);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            BrandResponse? brand = await _brandService.GetByIdAsync(id);

            if (brand == null)
            {
                return NotFound(new
                {
                    message = "Brand tidak ditemukan."
                });
            }

            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] BrandRequest request)
        {
            try
            {
                BrandResponse brand = await _brandService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = brand.BrandId },
                    brand
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] BrandRequest request)
        {
            try
            {
                BrandResponse? brand = await _brandService.UpdateAsync(id, request);

                if (brand == null)
                {
                    return NotFound(new
                    {
                        message = "Brand tidak ditemukan."
                    });
                }

                return Ok(brand);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool isDeleted = await _brandService.DeleteAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new
                    {
                        message = "Brand tidak ditemukan."
                    });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Brand tidak bisa didelete karena sedang digunakan oleh data lain."
                });
            }
        }
    }

}
