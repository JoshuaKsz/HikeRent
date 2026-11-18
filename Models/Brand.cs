using System.ComponentModel.DataAnnotations;

namespace HikeRent.Models
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }

        [Required]
        [MaxLength(100)]
        public string BrandName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<GearItem> GearItems { get; set; } = new List<GearItem>();
    }
}
