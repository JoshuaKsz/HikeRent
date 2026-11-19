using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.Category
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Nama kategori wajib diisi.")]
        [MaxLength(100, ErrorMessage = "Nama kategori maksimal 100 karakter.")]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
