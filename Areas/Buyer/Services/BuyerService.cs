using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocietyMng.Areas.Buyer.ModelView;
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

            if (!string.Equals(asset.Status?.Code, _appSetting.Asset_Status.AVAILABLE, StringComparison.OrdinalIgnoreCase))
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
            try
            {
                _logger.LogDebug("Attempting to cancel booking {BookingId} for user {UserId}", bookingId, userId);
                var strategy = _context.Database.CreateExecutionStrategy();

                BookingResult result = null;

                await strategy.ExecuteAsync(async () =>
                {
                    var booking = await _context.Bookings
                        .Include(b => b.Asset)
                            .ThenInclude(a => a.Status)
                            .ThenInclude(s => s.SystemCode)
                        .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

                    if (booking == null)
                    {
                        _logger.LogWarning("Booking {BookingId} not found for user {UserId}", bookingId, userId);
                        result = BookingResult.Failure("Booking not found or doesn't belong to you.");
                        return;
                    }

                 // null checks
                    if (booking.Asset == null || booking.Asset.Status == null)
                    {
                        _logger.LogWarning("Booking {BookingId} has invalid asset or status data", bookingId);
                        result = BookingResult.Failure("Invalid booking data. Please contact support.");
                        return;
                    }
                    var currentStatus = booking.Status?.Trim() ?? string.Empty;

                    if (currentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Booking {BookingId} is already cancelled", bookingId);
                        result = BookingResult.Failure("Booking is already cancelled.");
                        return;
                    }

                    if (currentStatus.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Attempted to cancel confirmed booking {BookingId}", bookingId);
                        result = BookingResult.Failure("Cannot cancel a confirmed booking. Please contact support.");
                        return;
                    }

                    if (currentStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Attempted to cancel rejected booking {BookingId}", bookingId);
                        result = BookingResult.Failure("This booking has already been rejected by the admin.");
                        return;
                    }

                    if (!currentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Attempted to cancel booking {BookingId} with status {Status}", bookingId, booking.Status);
                        result = BookingResult.Failure($"Cannot cancel booking with status: {booking.Status}");
                        return;
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        booking.Status = "Cancelled";
                        //case sensitivity handled
                        var availableStatus = await _context.SystemCodeItems
                            .Include(sci => sci.SystemCode)
                            .Where(sci => sci.SystemCode.Code == "Asset_Status" &&
                                         sci.Code.ToUpper() == _appSetting.Asset_Status.AVAILABLE.ToUpper())
                            .FirstOrDefaultAsync();

                        if (availableStatus == null)
                        {
                            _logger.LogError("Could not find AVAILABLE status in SystemCodeItems");
                            result = BookingResult.Failure("System error: Could not update asset status. Please contact support.");
                            return;
                        }
                        booking.Asset.StatusId = availableStatus.Id;

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Successfully cancelled booking {BookingId} for user {UserId}. Asset {AssetId} is now available.",
                            bookingId, userId, booking.Asset.Id);

                        result = BookingResult.SuccessResult(
                            "Booking cancelled successfully. The asset is now available for other buyers.",
                            booking.Id,
                            booking.BookingDate,
                            booking.Asset.Id,
                            booking.Asset.Description,
                            availableStatus.Code
                        );
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Concurrency conflict while cancelling booking {BookingId}", bookingId);
                        result = BookingResult.Failure("This booking was modified by another process. Please refresh and try again.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Database error while cancelling booking {BookingId}", bookingId);
                        result = BookingResult.Failure("A database error occurred while cancelling the booking. Please try again.");
                    }
                });

                return result ?? BookingResult.Failure("An unexpected error occurred during cancellation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CancelBookingAsync for booking {BookingId} and user {UserId}", bookingId, userId);
                return BookingResult.Failure($"An unexpected error occurred: {ex.Message}");
            }
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

        public async Task<(long minPrice, long maxPrice)> GetPriceRangeAsync()
        {
            if (!await _context.Assets.AnyAsync())
            {
                return (0, 100000000);
            }

            var minPrice = (long)await _context.Assets.MinAsync(a => a.Price);
            var maxPrice = (long)await _context.Assets.MaxAsync(a => a.Price);
            return (minPrice, maxPrice);
        }
    }
}