using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeRent.Models
{
    public class RentalDetail
    {
        [Key]
        public int RentalDetailId { get; set; }

        [Required]
        public int RentalId { get; set; }

        [Required]
        public int GearItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal DailyPrice { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ReplacementPrice { get; set; }

        public int RentalDays { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal {  get; set; }

        public Rental? Rental { get; set; }

        public GearItem? GearItem { get; set; }

        public ReturnRecord? ReturnRecord { get; set; }
    }
}
