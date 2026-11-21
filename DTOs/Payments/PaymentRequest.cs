using HikeRent.Enums;
using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.Payments
{
    public class PaymentRequest
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
