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
            return await _context.Assets
                .Include(a => a.Block)
                .Include(a => a.PropertyType)
                .Include(a => a.Status)
                .Where(a => a.Status.Code == _appSetting.Asset_Status.AVAILABLE)
                .ToListAsync();
        }

        public async Task<Asset> GetAssetByIdAsync(int assetId)
        {
            return await _context.Assets
                .Include(a => a.Block)
                .Include(a => a.PropertyType)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == assetId)
                ?? throw new KeyNotFoundException($"Asset with Id {assetId} not found.");
        }

        public async Task<BookingResult> BookAssetAsync(int userId, int assetId)
        {
            var asset = await _context.Assets
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == assetId);

            if (asset == null)
                return BookingResult.Failure("Asset not found.");

            if (asset.Status.Code != _appSetting.Asset_Status.AVAILABLE)
                return BookingResult.Failure("Asset is not available for booking.");

            var booking = new Booking
            {
                UserId = userId,
                AssetId = assetId,
                BookingDate = DateTime.UtcNow,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var bookedStatus = await _context.SystemCodeItems
                .FirstOrDefaultAsync(sci => sci.SystemCode.Code == "Asset_Status" && sci.Code == _appSetting.Asset_Status.BOOKED);

            if (bookedStatus == null)
                return BookingResult.Failure("Unable to find BOOKED status code.");

            asset.StatusId = bookedStatus.Id;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return BookingResult.SuccessResult(
                "Booking created successfully.",
                booking.Id,
                booking.BookingDate,
                asset.Id,
                asset.Description,
                bookedStatus.Code
            );
        }

        public async Task<BookingResult> CancelBookingAsync(int userId, int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Status)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
                return BookingResult.Failure("Booking not found for this user.");

            if (booking.Status == "Cancelled")
                return BookingResult.Failure("Booking is already cancelled.");

            booking.Status = "Cancelled";

            var availableStatus = await _context.SystemCodeItems
                .FirstOrDefaultAsync(sci => sci.SystemCode.Code == "Asset_Status" && sci.Code == _appSetting.Asset_Status.AVAILABLE);

            if (availableStatus == null)
                return BookingResult.Failure("Unable to find AVAILABLE status code.");

            booking.Asset.StatusId = availableStatus.Id;
            await _context.SaveChangesAsync();

            return BookingResult.SuccessResult(
                "Booking cancelled successfully.",
                booking.Id,
                booking.BookingDate,
                booking.Asset.Id,
                booking.Asset.Description,
                availableStatus.Code
            );
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Block)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.PropertyType)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Status)
                .ToListAsync();
        }
    }
}

//public async Task<List<Asset>> GetAllAssetsAsync()
//{
//    _logger.LogDebug("Fetching all assets from database");
//    var assets = await _context.Assets
//        .Include(a => a.Block)
//        .Include(a => a.PropertyType)
//        .Include(a => a.Status)
//        .ToListAsync();
//    _logger.LogInformation("Retrieved {AssetCount} assets", assets.Count);
//    return assets;
//}

//        public async Task<Asset> GetAssetByIdAsync(int assetId)
//        {
//            _logger.LogDebug("Fetching specific assets from database");
//            var assets = await _context.Assets
//                .Include(a => a.Block)
//                .Include(a => a.PropertyType)
//                .Include(a => a.Status)
//                .FirstOrDefaultAsync(a => a.Id == assetId);
//            _logger.LogInformation("Retrieved {id} assets", assetId);

//            return assets;
//        }

//        public async Task<BookingResult> BookAssetAsync(int userId, int assetId)
//        {
//            _logger.LogInformation("BOOKING STARTED - User: {UserId}, Asset: {AssetId}", userId, assetId);

//            using var transaction = await _context.Database.BeginTransactionAsync();
//            try
//            {
//                // 1. Retrieve asset with detailed logging
//                var asset = await _context.Assets
//                    .Include(a => a.Status)
//                    .FirstOrDefaultAsync(a => a.Id == assetId);

//                _logger.LogDebug("Asset details - ID: {Id}, Name: {Name}, Status: {StatusCode} ({StatusDesc})",
//                    asset?.Id, asset?.Id, asset?.Status?.Code, asset?.Status?.Description);

//                if (asset == null)
//                {
//                    _logger.LogError("ASSET NOT FOUND - Asset ID: {AssetId}", assetId);
//                    return new BookingResult { Success = false, ErrorMessage = "Asset not found" };
//                }

//                // 2. Check asset availability
//                _logger.LogDebug("Checking availability - Current: {Current}, Required: {Required}",
//                    asset.Status?.Code, _appSetting.Asset_Status.AVAILABLE);

//                if (asset.Status.Code != _appSetting.Asset_Status.AVAILABLE)
//                {
//                    _logger.LogWarning("ASSET UNAVAILABLE - Current status: {Status}", asset.Status.Code);
//                    return new BookingResult
//                    {
//                        Success = false,
//                        ErrorMessage = $"Asset is not available (Status: {asset.Status.Description})"
//                    };
//                }

//                // 3. Check for existing bookings
//                var existingBooking = await _context.Bookings
//                    .FirstOrDefaultAsync(b => b.UserId == userId && b.AssetId == assetId &&
//                                        (b.Status == "Pending" || b.Status == "Confirmed"));

//                _logger.LogDebug("Existing booking check - Found: {HasExisting}", existingBooking != null);

//                if (existingBooking != null)
//                {
//                    _logger.LogWarning("DUPLICATE BOOKING - Existing ID: {BookingId}", existingBooking.Id);
//                    return new BookingResult
//                    {
//                        Success = false,
//                        ErrorMessage = "You already have an active booking for this asset"
//                    };
//                }

//                // 4. Create new booking
//                var booking = new Booking
//                {
//                    UserId = userId,
//                    AssetId = assetId,
//                    BookingDate = DateTime.Now,
//                    Status = "Pending",
//                    CreatedAt = DateTime.Now
//                };

//                _logger.LogInformation("Creating new booking record...");
//                await _context.Bookings.AddAsync(booking);

//                // 5. Update asset status
//                var bookedStatus = await _context.SystemCodeItems
//                    .FirstOrDefaultAsync(s => s.Code == _appSetting.Asset_Status.BOOKED);

//                _logger.LogDebug("Updating asset status to: {Status}", bookedStatus?.Code);

//                if (bookedStatus != null)
//                {
//                    asset.StatusId = bookedStatus.Id;
//                }

//                // 6. Save changes
//                _logger.LogInformation("Saving changes to database...");
//                await _context.SaveChangesAsync();
//                await transaction.CommitAsync();

//                _logger.LogInformation("BOOKING SUCCESSFUL - Booking ID: {BookingId}", booking.Id);
//                return new BookingResult { Success = true, BookingId = booking.Id };
//            }
//            catch (Exception ex)
//            {
//                await transaction.RollbackAsync();
//                _logger.LogError(ex, "BOOKING FAILED - User: {UserId}, Asset: {AssetId}", userId, assetId);
//                return new BookingResult
//                {
//                    Success = false,
//                    ErrorMessage = "A system error occurred while processing your booking"
//                };
//            }
//        }
//        public async Task<BookingResult> CancelBookingAsync(int userId, int bookingId)
//        {
//            using var transaction = await _context.Database.BeginTransactionAsync();

//            try
//            {
//                var booking = await _context.Bookings
//                    .Include(b => b.Asset)
//                    .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

//                if (booking == null)
//                {
//                    return new BookingResult { Success = false, ErrorMessage = "Booking not found" };
//                }

//                if (booking.Status == "Cancelled")
//                {
//                    return new BookingResult { Success = false, ErrorMessage = "Booking is already cancelled" };
//                }

//                booking.Status = "Cancelled";

//                var availableStatus = await _context.SystemCodeItems
//                    .FirstOrDefaultAsync(s => s.Code == _appSetting.Asset_Status.AVAILABLE);

//                if (availableStatus != null)
//                {
//                    booking.Asset.StatusId = availableStatus.Id;
//                }

//                await _context.SaveChangesAsync();
//                await transaction.CommitAsync();

//                _logger.LogInformation("Booking {BookingId} cancelled successfully by user {UserId}", bookingId, userId);
//                return new BookingResult { Success = true };
//            }
//            catch (Exception ex)
//            {
//                await transaction.RollbackAsync();
//                _logger.LogError(ex, "Error cancelling booking {BookingId} for user {UserId}", bookingId, userId);
//                return new BookingResult { Success = false, ErrorMessage = "An error occurred while cancelling the booking" };
//            }
//        }

//        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
//        {
//            return await _context.Bookings
//                .Include(b => b.Asset)
//                    .ThenInclude(a => a.Block)
//                .Include(b => b.Asset)
//                    .ThenInclude(a => a.PropertyType)
//                .Include(b => b.Asset)
//                    .ThenInclude(a => a.Status)
//                .Where(b => b.UserId == userId)
//                .OrderByDescending(b => b.CreatedAt)
//                .ToListAsync();
//        }


//    }

