using HikeRent.Data;
using HikeRent.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Seeders
{
    public class StaffSeeder
    {
        public static async Task SeedAsync(
            HikeRentDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            string email = "staff@gmail.com";
            string password = "staffsuper";

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Staff seed configuration is incomplete.");
            }

            email = email.Trim().ToLowerInvariant();

            // Check whether staff already exists
            var existingUser = await context.Users
                .AnyAsync(u => u.Email == email);

            if (existingUser)
            {
                return;
            }

            var staff = new User
            {
                FullName = "Initial Staff",
                Email = email,
                Role = "Staff",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            staff.PasswordHash = passwordHasher.HashPassword(
                staff,
                password
            );

            await context.Users.AddAsync(staff);
            await context.SaveChangesAsync();
        }
    }
}
