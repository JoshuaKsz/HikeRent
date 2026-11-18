using System.ComponentModel.DataAnnotations;

namespace HikeRent.Models
{
    public class ActivityLog
    {
        [Key]
        public int ActivityLogId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Activity {  get; set; } = string.Empty;

        public DateTime ActivityDate { get; set; }

        public User? User { get; set; }
    }
}
