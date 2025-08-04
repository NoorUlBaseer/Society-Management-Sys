using SocietyMng.Areas.Buyer.DTOs;
using SocietyMng.Data;
using SocietyMng.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SocietyMng.Services.Interfaces;

namespace SocietyMng.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly AppDbContext _context;

        public BuyerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Profile> GetProfileAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new Profile
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                RoleId = user.RoleId.ToString(),
                Role = user.Role?.Description,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateProfileAsync(Profile profile)
        {
            var user = await _context.Users.FindAsync(profile.Id);
            if (user == null)
                return false;

            // Update editable fields
            user.Email = profile.Email;
            user.PhoneNumber = profile.PhoneNumber;

            // Update password if provided
            if (!string.IsNullOrEmpty(profile.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(profile.NewPassword);
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}