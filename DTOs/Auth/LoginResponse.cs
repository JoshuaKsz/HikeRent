namespace HikeRent.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public UserSummary User { get; set; } = new UserSummary();
    }
}
