using System.ComponentModel.DataAnnotations;

namespace HikeRent.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<GearItem> GearItems { get; set; } = new List<GearItem>();
    }
}
