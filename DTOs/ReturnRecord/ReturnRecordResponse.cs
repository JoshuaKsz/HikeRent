
namespace HikeRent.DTOs.ReturnRecord
{
    public class ReturnRecordResponse
    {
        public int ReturnRecordId { get; set; }

        public int UserId { get; set; }

        public int QuantityReturned { get; set; }

        public int DamagedQuantity { get; set; }

        public int LostQuantity { get; set; }

        public string? Notes { get; set; }

        public decimal LateFee { get; set; }

        public decimal DamageFee { get; set; }

        public decimal LostItemFee { get; set; }

        public DateTime ActualReturnDate { get; set; }

    }
}
