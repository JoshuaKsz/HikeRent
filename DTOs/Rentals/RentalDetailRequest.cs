    using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.Rentals
{
    public class RentalDetailRequest
    {
        [Range(1, int.MaxValue)]
        public int GearItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
