using HikeRent.Models;

namespace HikeRent.Services.Interfaces
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
    }
}
