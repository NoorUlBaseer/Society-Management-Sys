using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocietyMng.Areas.Buyer.DTOs;
using SocietyMng.Configurations;
using SocietyMng.Data;
using SocietyMng.Data.Entities;
using SocietyMng.Services.Interfaces;

namespace SocietyMng.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BuyerService> _logger;
        private readonly AppSettings _appSetting;

        public BuyerService(AppDbContext context, ILogger<BuyerService> logger, IOptions<AppSettings> appSets)
        {
            _context = context;
            _logger = logger;
            _appSetting = appSets.Value;
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

            user.Email = profile.Email;
            user.PhoneNumber = profile.PhoneNumber;
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

        public async Task<List<Asset>> GetAllAssetsAsync()
        {
            _logger.LogDebug("Fetching all assets from database");
            var assets = await _context.Assets
                .Include(a => a.Block)
                .Include(a => a.PropertyType)
                .Include(a => a.Status)
                .ToListAsync();
            _logger.LogInformation("Retrieved {AssetCount} assets", assets.Count);
            return assets;
        }

        public async Task<Asset> GetAssetByIdAsync(int assetId)
        {
            _logger.LogDebug("Fetching specific assets from database");
            var assets = await _context.Assets
                .Include(a => a.Block)
                .Include(a => a.PropertyType)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == assetId);
            _logger.LogInformation("Retrieved {id} assets", assetId);
           
            return assets;
        }

        public async Task<BookingResult> BookAssetAsync(int userId, int assetId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var asset = await _context.Assets
                    .Include(a => a.Status)
                    .FirstOrDefaultAsync(a => a.Id == assetId);

                if (asset == null)
                {
                    return new BookingResult { Success = false, ErrorMessage = "Asset not found" };
                }

                if (asset.Status.Code != _appSetting.Asset_Status.AVAILABLE)
                {
                    return new BookingResult { Success = false, ErrorMessage = "Asset is not available for booking" };
                }

                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.AssetId == assetId &&
                                        (b.Status == "Pending" || b.Status == "Confirmed"));

                if (existingBooking != null)
                {
                    return new BookingResult { Success = false, ErrorMessage = "You already have a booking for this asset" };
                }

                var booking = new Booking
                {
                    UserId = userId,
                    User= await _context.Users.FindAsync(userId),
                    AssetId = assetId,
                    Asset = asset,
                    BookingDate = DateTime.Now,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                await _context.Bookings.AddAsync(booking);
                _logger.LogDebug("Asset added to context. About to save changes.");

                var bookedStatus = await _context.SystemCodeItems
                    .FirstOrDefaultAsync(s => s.Code == _appSetting.Asset_Status.BOOKED);

                if (bookedStatus != null)
                {
                    asset.StatusId = bookedStatus.Id;
                }

                var result= await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Asset {AssetId} booked successfully by user {UserId}", assetId, userId);
                return new BookingResult { Success = true, BookingId = booking.Id };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error booking asset {AssetId} for user {UserId}", assetId, userId);
                return new BookingResult { Success = false, ErrorMessage = "An error occurred while booking the asset" };
            }
        }

        public async Task<BookingResult> CancelBookingAsync(int userId, int bookingId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Asset)
                    .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

                if (booking == null)
                {
                    return new BookingResult { Success = false, ErrorMessage = "Booking not found" };
                }

                if (booking.Status == "Cancelled")
                {
                    return new BookingResult { Success = false, ErrorMessage = "Booking is already cancelled" };
                }

                booking.Status = "Cancelled";

                var availableStatus = await _context.SystemCodeItems
                    .FirstOrDefaultAsync(s => s.Code == _appSetting.Asset_Status.AVAILABLE);

                if (availableStatus != null)
                {
                    booking.Asset.StatusId = availableStatus.Id;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Booking {BookingId} cancelled successfully by user {UserId}", bookingId, userId);
                return new BookingResult { Success = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling booking {BookingId} for user {UserId}", bookingId, userId);
                return new BookingResult { Success = false, ErrorMessage = "An error occurred while cancelling the booking" };
            }
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Block)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.PropertyType)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Status)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        //public async Task<List<Complaint>> GetUserComplaintsAsync(int userId)
        //{
        //    return await _context.Complaints
        //        .Include(c => c.Asset)
        //        .Include(c => c.Booking)
        //        .Where(c => c.UserId == userId)
        //        .OrderByDescending(c => c.CreatedAt)
        //        .ToListAsync();
        //}
    }

    public class BookingResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int? BookingId { get; set; }
    }
}