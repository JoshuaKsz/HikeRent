using HikeRent.DTOs.GearItems;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/gearitem")]
    [Authorize]
    public class GearItemController : ControllerBase
    {
        private readonly IGearItemService _gearItemService;
        private readonly IFileStorageService _fileStorageService;

        public GearItemController(IGearItemService gearItemService, IFileStorageService fileStorageService)
        {
            _gearItemService = gearItemService;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<GearItemResponse> gearItems = await _gearItemService.GetAllAsync();

            return Ok(gearItems);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            GearItemResponse? gearItem = await _gearItemService.GetByIdAsync(id);

            if (gearItem == null)
            {
                return NotFound(new
                {
                    message = "Data peralatan tidak ditemukan."
                });
            }

            return Ok(gearItem);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] GearItemRequest request,
            IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    request.ImageUrl = await _fileStorageService.SaveGearImageAsync(imageFile);
                }

                GearItemResponse gearItem =
                    await _gearItemService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = gearItem.GearItemId },
                    gearItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] GearItemRequest request,
            IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    request.ImageUrl = await _fileStorageService.SaveGearImageAsync(imageFile);
                }

                GearItemResponse? gearItem =
                    await _gearItemService.UpdateAsync(id, request);

                if (gearItem == null)
                {
                    return NotFound(new
                    {
                        message = "Data peralatan tidak ditemukan."
                    });
                }

                return Ok(gearItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
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
                bool isDeleted =
                    await _gearItemService.DeleteAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new
                    {
                        message = "Data peralatan tidak ditemukan."
                    });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Peralatan tidak dapat dihapus karena masih digunakan dalam transaksi penyewaan."
                });
            }
        }

    }
}

