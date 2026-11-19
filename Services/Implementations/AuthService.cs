using HikeRent.DTOs.Auth;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HikeRent.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
                );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            (string Token, DateTime ExpiresAt) jwtResult = _jwtService.GenerateToken(user);

            return new LoginResponse
            {
                Token = jwtResult.Token,
                ExpiresAt = jwtResult.ExpiresAt,

                User = new DTOs.UserSummary
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }
    }
}
