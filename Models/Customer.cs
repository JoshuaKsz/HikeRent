using System.ComponentModel.DataAnnotations;

namespace HikeRent.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Address {  get; set; }

        [MaxLength(50)]
        public string? IdentityNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
