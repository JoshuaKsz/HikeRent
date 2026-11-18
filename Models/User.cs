using System.ComponentModel.DataAnnotations;

namespace HikeRent.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Role {  get; set; } = "Staff";

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<ReturnRecord> ReturnRecords { get; set; } = new List<ReturnRecord>();

        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
