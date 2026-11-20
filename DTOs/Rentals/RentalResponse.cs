namespace HikeRent.DTOs.Rentals
{
    public class RentalResponse
    {
        public int RentalId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string RentalCode { get; set; } = string.Empty;

        public DateTime RentalDate { get; set; }

        public DateOnly PlannedStartDate { get; set; }

        public DateOnly PlannedReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalRentalFee { get; set; }

        public decimal TotalLateFee { get; set; }

        public decimal TotalDamageFee { get; set; }

        public decimal TotalLostItemFee { get; set; }

        public decimal GrandTotal { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<RentalDetailResponse> Details { get; set; } = new();
    }
}
