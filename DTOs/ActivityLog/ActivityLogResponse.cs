namespace HikeRent.DTOs.ActivityLog
{
    public class ActivityLogResponse
    {
        public int ActivityLogId { get; set; }

        public int UserId { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public string Activity { get; set; } = string.Empty;

        public DateTime ActivityDate { get; set; }
    }
}
