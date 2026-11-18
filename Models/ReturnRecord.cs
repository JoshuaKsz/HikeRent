using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeRent.Models
{
    public class ReturnRecord
    {
        [Key]
        public int ReturnRecordId { get; set; }

        [Required]
        public int RentalDetailId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int QuantityReturned { get; set; }

        public int DamagedQuantity { get; set; } = 0;

        public int LostQuantity { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        public string Condition { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal RentalFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal LateFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal LostItemFee { get; set; }

        public DateTime ActualReturnDate { get; set; }

        public RentalDetail? RentalDetail { get; set; }

        public User? User { get; set; }
    }
}
