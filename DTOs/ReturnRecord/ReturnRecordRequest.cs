using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.ReturnRecord
{
    public class ReturnRecordRequest
    {
        [Required]
        public int RentalDetailId { get; set; }

        //public int UserId { get; set; }
        [Range(0, int.MaxValue)]
        public int QuantityReturned { get; set; }
        [Range(0, int.MaxValue)]
        public int DamagedQuantity { get; set; }
        [Range(0, int.MaxValue)]
        public int LostQuantity { get; set; }

        public string Notes { get; set; } = string.Empty;

        public DateTime ActualReturnDate { get; set; }
    }
}
