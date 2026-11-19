using HikeRent.DTOs.Auth;

namespace HikeRent.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
