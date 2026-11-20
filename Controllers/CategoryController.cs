using HikeRent.DTOs.Category;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/category")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<CategoryResponse> categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            CategoryResponse? category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Kategori tidak ditemukan."
                });
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CategoryRequest request)
        {
            try
            {
                CategoryResponse category = await _categoryService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = category.CategoryId },
                    category
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
            [FromBody] CategoryRequest request)
        {
            try
            {
                CategoryResponse? category = await _categoryService.UpdateAsync(id, request);

                if (category == null)
                {
                    return NotFound(new
                    {
                        message = "Kategori tidak ditemukan."
                    });
                }

                return Ok(category);
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
                bool isDeleted = await _categoryService.DeleteAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new
                    {
                        message = "Kategori tidak ditemukan."
                    });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Kategori tidak bisa didelete karena digunakan oleh data lain."
                });
            }
        }
    }
}
