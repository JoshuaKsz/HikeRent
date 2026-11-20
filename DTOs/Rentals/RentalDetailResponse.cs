namespace HikeRent.DTOs.Rentals
{
    public class RentalDetailResponse
    {
        public int RentalDetailId { get; set; }

        public int GearItemId { get; set; }

        public string GearName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal DailyPrice { get; set; }

        public decimal ReplacementPrice { get; set; }

        public int RentalDays { get; set; }

        public decimal Subtotal { get; set; }
    }
}
