using HikeRent.Models;
using HikeRent.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HikeRent.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            string? secretKey = _configuration["JwtSettings:SecretKey"];
            string? issuer = _configuration["JwtSettings:Issuer"];
            string? audience = _configuration["JwtSettings:Audience"];
            int expiration = _configuration.GetValue<int>("JwtSettings:ExpireMinutes");

            if (string.IsNullOrWhiteSpace(secretKey) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                expiration <= 0)
            {
                throw new InvalidOperationException("JWT configuration is incomplete or invalid.");
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey must contain at least 32 bytes.");
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(keyBytes);

            SigningCredentials credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
                );

            DateTime now = DateTime.UtcNow;
            DateTime expiresAt = now.AddMinutes(expiration);

            List<Claim> claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                    ),

                new Claim(
                    ClaimTypes.Name,
                    user.FullName
                    ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                    ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role
                    ),
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials
                );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiresAt);
        }
    }
}
