using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeRent.Models
{
    public class GearItem
    {
        [Key]
        public int GearItemId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [MaxLength(150)]
        public string GearName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal DailyPrice { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ReplacementPrice { get; set; }

        public int TotalStock { get; set; } = 0;

        public int MaintenanceStock { get; set; } = 0;

        public int DamagedStock { get; set; } = 0;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public Category? Category { get; set; }

        public Brand? Brand { get; set; }

        public ICollection<RentalDetail> RentalDetails { get; set; } = new List<RentalDetail>();
    }
}
