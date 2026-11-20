using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.ActivityLog
{
    public class ActivityLogRequest
    {
        [Required(ErrorMessage = "Aktivitas wajib diisi.")]
        public string Activity { get; set; } = string.Empty;
    }
}
