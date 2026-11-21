namespace HikeRent.DTOs.Payments
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; }

        public string PaymentCode { get; set; } = string.Empty;

        public int RentalId { get; set; }

        public string RentalCode { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal GrandTotal { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
