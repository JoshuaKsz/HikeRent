using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.Brands
{
    public class BrandRequest
    {
        [Required]
        [StringLength(100)]
        public string BrandName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
