using HikeRent.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeRent.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RentalCode { get; set; } = string.Empty;

        public DateTime RentalDate { get; set; }

        public DateOnly PlannedStartDate { get; set; }

        public DateOnly PlannedReturnDate { get; set; }

        [Required]
        [MaxLength(30)]
        public RentalStatus Status { get; set; } = RentalStatus.Ongoing;

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalRentalFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalLateFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalDamageFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalLostItemFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal GrandTotal {  get; set; }

        [Required]
        [MaxLength(30)]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public DateTime CreatedAt { get; set; }

        public Customer? Customer { get; set; }

        public User? User { get; set; }

        public ICollection<RentalDetail> RentalDetails { get; set; } = new List<RentalDetail>();

        public Payment? Payment { get; set; }


    }
}
