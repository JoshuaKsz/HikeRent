using HikeRent.DTOs.Brands;
using HikeRent.DTOs.Category;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;

namespace HikeRent.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            IEnumerable<Category> categories = await _categoryRepository.GetAllAsync();

            return categories.Select(MapToResponse);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return MapToResponse(category);
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
        {
            string categoryName = request.CategoryName.Trim();

            bool existingCategory = await _categoryRepository.CategoryNameExistsAsync(categoryName);

            if (existingCategory)
            {
                throw new InvalidOperationException("Nama kategori sudah digunakan.");
            }

            Category category = new Category
            {
                CategoryName = categoryName,
                Description = request.Description?.Trim()
            };

            await _categoryRepository.AddAsync(category);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            string categoryName = request.CategoryName.Trim();

            Category? existingCategory = await _categoryRepository.GetByNameAsync(categoryName);

            if (existingCategory != null && existingCategory.CategoryId != id)
            {
                throw new InvalidOperationException("Nama kategori sudah digunakan.");
            }

            category.CategoryName = categoryName;
            category.Description = request.Description?.Trim();

            _categoryRepository.Update(category);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category == null) { return false; }

            _categoryRepository.Delete(category);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static CategoryResponse MapToResponse(Category category)
        {
            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }
    }

}
