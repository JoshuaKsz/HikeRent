using HikeRent.DTOs.Auth;
using HikeRent.Models;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HikeRent.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login gagal: Email {Email} tidak ditemukan atau tidak aktif", email);
                return null;
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
                );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login gagal: Password salah untuk email {Email}", email);
                return null;
            }

            (string Token, DateTime ExpiresAt) jwtResult = _jwtService.GenerateToken(user);

            _logger.LogInformation("User {Email} berhasil login", email);

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
