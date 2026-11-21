using HikeRent.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeRent.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int RentalId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public Rental? Rental { get; set; }

        public User? User { get; set; }
    }
}
