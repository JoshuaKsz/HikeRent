using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.Rentals
{
    public class RentalRequest : IValidatableObject
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required]
        public DateOnly PlannedStartDate { get; set; }

        [Required]
        public DateOnly PlannedReturnDate { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Minimal harus ada satu peralatan yang disewa.")]
        public List<RentalDetailRequest> Details { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (PlannedStartDate == default)
            {
                yield return new ValidationResult(
                    "Tanggal mulai sewa wajib diisi.",
                    new[] { nameof(PlannedStartDate) });
            }

            if (PlannedReturnDate == default)
            {
                yield return new ValidationResult(
                    "Tanggal pengembalian wajib diisi.",
                    new[] { nameof(PlannedReturnDate) });
            }

            if (PlannedReturnDate < PlannedStartDate)
            {
                yield return new ValidationResult(
                    "Tanggal pengembalian tidak boleh lebih awal dari tanggal mulai sewa.",
                    new[] { nameof(PlannedReturnDate) });
            }

            if (Details == null || Details.Count == 0)
            {
                yield return new ValidationResult(
                    "Minimal harus ada satu peralatan yang disewa.",
                    new[] { nameof(Details) });
            }
        }
    }
}
