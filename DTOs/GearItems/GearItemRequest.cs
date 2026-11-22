using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.GearItems
{
    public class GearItemRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [StringLength(150)]
        public string GearName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0.01, 9999999999.99)]
        public decimal DailyPrice { get; set; }

        [Range(0.01, 9999999999.99)]
        public decimal ReplacementPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalStock { get; set; }

        [Range(0, int.MaxValue)]
        public int DamagedStock { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
