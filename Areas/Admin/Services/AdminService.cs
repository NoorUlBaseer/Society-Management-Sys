using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocietyMng.Areas.Admin.ModelView;
using SocietyMng.Configurations;
using SocietyMng.Data;
using SocietyMng.Data.Entities;

namespace SocietyMng.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminService(AppDbContext context, ILogger<AdminService> logger, IOptions<AppSettings> appSets, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _appSettings = appSets.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        // User Management
        public async Task<List<User>> GetAllUsersAsync()
        {
            _logger.LogDebug("Fetching all non-admin users from database");
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Code != _appSettings.User_Role.Admin)
                .ToListAsync();
            _logger.LogInformation("Retrieved {UserCount} non-admin users", users.Count);
            return users;
        }

        public async Task BlockUserAsync(int userId)
        {
            _logger.LogDebug("Attempting to block/unblock user ID: {UserId}", userId);
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                _logger.LogInformation("User ID: {UserId} status changed to {Status}", userId, user.IsActive ? "Active" : "Inactive");
            }
            else
            {
                _logger.LogWarning("User ID: {UserId} not found for blocking/unblocking", userId);
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            _logger.LogDebug("Attempting to delete user ID: {UserId}", userId);
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted user ID: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("User ID: {UserId} not found for deletion", userId);
            }
        }

        public async Task AddAssetAsync(AssetCreateView model)
        {
            _logger.LogDebug("Creating new asset: {Description}", model.Description);

            try
            {
                var defaultStatus = await _context.SystemCodeItems
                    .Include(s => s.SystemCode)
                    .FirstOrDefaultAsync(s => s.SystemCode.Code == "Asset_Status" && s.Code == "Available");

                if (defaultStatus == null)
                {
                    throw new Exception("Default status 'Available' not found in SystemCodeItems.");
                }

                var asset = new Asset
                {
                    Description = model.Description,
                    Address = model.Address,
                    PlotNumber = model.PlotNumber,
                    ImagePath = model.ImagePath,
                    Price = model.Price,
                    BlockId = model.BlockId,
                    PropertyTypeId = model.PropertyTypeId,
                    StatusId = defaultStatus.Id,
                    DateUploaded = DateTime.UtcNow
                };

                _logger.LogDebug("Asset entity created. About to add to context.");

                await _context.Assets.AddAsync(asset);

                _logger.LogDebug("Asset added to context. About to save changes.");

                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created asset ID: {AssetId}. SaveChanges result: {Result}",
                    asset.Id, result);

                if (result == 0)
                {
                    _logger.LogWarning("SaveChanges returned 0, indicating no changes were saved to database");
                    throw new InvalidOperationException("No changes were saved to the database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAssetAsync for asset: {Description}", model.Description);
                throw;
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

        public async Task<Asset> UpdateAssetAsync(int id, AssetUpdateView model)
        {
            _logger.LogDebug("Updating asset ID: {AssetId}", id);

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Asset with ID: {AssetId} not found for update", id);
                throw new KeyNotFoundException($"Asset with ID {id} not found");
            }

            asset.Description = model.Description ?? asset.Description;
            asset.Address = model.Address ?? asset.Address;
            asset.PlotNumber = model.PlotNumber ?? asset.PlotNumber;
            asset.Price = model.Price ?? asset.Price;
            asset.BlockId = model.BlockId ?? asset.BlockId;
            asset.PropertyTypeId = model.PropertyTypeId ?? asset.PropertyTypeId;
            asset.StatusId = model.StatusId ?? asset.StatusId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated asset ID: {AssetId}", id);
            return asset;
        }

        public async Task DeleteAssetAsync(int id)
        {
            _logger.LogDebug("Deleting asset ID: {AssetId}", id);

            var asset = await _context.Assets
                .Include(a => a.Bookings) // Include related bookings
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
            {
                _logger.LogWarning("Asset with ID: {AssetId} not found for deletion", id);
                throw new KeyNotFoundException($"Asset with ID {id} not found");
            }

            var imagePath = asset.ImagePath;

            try
            {
                // First delete all related bookings
                if (asset.Bookings != null && asset.Bookings.Any())
                {
                    _context.Bookings.RemoveRange(asset.Bookings);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Deleted {BookingCount} bookings related to asset ID: {AssetId}",
                        asset.Bookings.Count, id);
                }

                // Then remove the asset
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted asset ID: {AssetId} from database", id);

                // Finally remove associated file if exists
                if (!string.IsNullOrEmpty(imagePath))
                {
                    await DeleteImageFileAsync(imagePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset ID: {AssetId}", id);
                throw;
            }
        }

        private async Task DeleteImageFileAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    _logger.LogDebug("No image path provided for deletion");
                    return;
                }

                // converting web path to physical, rmv /
                var relativePath = imagePath.TrimStart('/');

                // /->\ for windows path
                relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

                var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                _logger.LogDebug("Attempting to delete image file: {PhysicalPath}", physicalPath);

                if (File.Exists(physicalPath))
                {
                    await Task.Run(() => File.Delete(physicalPath));
                    _logger.LogInformation("Successfully deleted image file: {PhysicalPath}", physicalPath);
                }
                else
                {
                    _logger.LogWarning("Image file not found for deletion: {PhysicalPath}", physicalPath);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access denied when trying to delete image file: {ImagePath}", imagePath);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogError(ex, "Directory not found when trying to delete image file: {ImagePath}", imagePath);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found when trying to delete image file: {ImagePath}", imagePath);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "IO error when trying to delete image file: {ImagePath}", imagePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when trying to delete image file: {ImagePath}", imagePath);
            }
        }

        public async Task<List<BookedAssetView>> GetAllBookedAssetsAsync()
        {
            _logger.LogDebug("Fetching all booked assets from database");

            var bookedAssets = await _context.Bookings
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Block)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.PropertyType)
                .Include(b => b.Asset)
                    .ThenInclude(a => a.Status)
                .Include(b => b.User)
                .Where(b => b.Status != "Cancelled")
                .Select(b =>
                new BookedAssetView
                {
                    BookingId = b.Id,
                    BookingDate = b.BookingDate,
                    BookingStatus = b.Status,
                    AssetId = b.Asset.Id,
                    AssetDescription = b.Asset.Description,
                    AssetBlock = b.Asset.Block.Description,
                    AssetType = b.Asset.PropertyType.Description,
                    AssetPrice = b.Asset.Price,
                    UserId = b.User.Id,
                    UserName = b.User.FullName,
                    UserEmail = b.User.Email,
                    UserPhone = b.User.PhoneNumber
                })
                .OrderByDescending(b => b.BookingDate)
                .ThenBy(b => b.BookingStatus == "Pending" ? 0 : 1) 
                .ToListAsync();

            _logger.LogInformation("Retrieved {BookingCount} booked assets", bookedAssets.Count);
            return bookedAssets;
        }

        public async Task<bool> VerifyBookingAsync(int bookingId)
        {
            _logger.LogDebug("Verifying booking ID: {BookingId}", bookingId);

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Asset)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                {
                    _logger.LogWarning("Booking ID: {BookingId} not found", bookingId);
                    return false;
                }

                //getting status 
                var confirmedStatus = await _context.SystemCodeItems
                    .Include(s => s.SystemCode)
                    .FirstOrDefaultAsync(s => s.SystemCode.Code == "Asset_Status" && s.Code == _appSettings.Asset_Status.SOLD);

                if (confirmedStatus == null)
                {
                    _logger.LogError("SOLD status not found in SystemCodeItems");
                    throw new Exception("SOLD status configuration not found");
                }
                //on verify-> asset=Sold, booking=Confirmed
                booking.Status = "Confirmed";
                booking.Asset.StatusId = confirmedStatus.Id;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully verified booking ID: {BookingId}. Asset ID: {AssetId} status changed to Sold",
                    bookingId, booking.Asset.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying booking ID: {BookingId}", bookingId);
                return false;
            }
        }

        public async Task<bool> RejectBookingAsync(int bookingId)
        {
            _logger.LogDebug("Rejecting booking ID: {BookingId}", bookingId);

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Asset)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                {
                    _logger.LogWarning("Booking ID: {BookingId} not found", bookingId);
                    return false;
                }

                //fetch status
                var availableStatus = await _context.SystemCodeItems
                    .Include(s => s.SystemCode)
                    .FirstOrDefaultAsync(s => s.SystemCode.Code == "Asset_Status" && s.Code == _appSettings.Asset_Status.AVAILABLE);

                if (availableStatus == null)
                {
                    _logger.LogError("AVAILABLE status not found in SystemCodeItems");
                    throw new Exception("AVAILABLE status configuration not found");
                }
                // on reject-> Asset= available, booking= rejected
                booking.Status = "Rejected";
                booking.Asset.StatusId = availableStatus.Id;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully rejected booking ID: {BookingId}. Asset ID: {AssetId} status changed to Available",
                    bookingId, booking.Asset.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting booking ID: {BookingId}", bookingId);
                return false;
            }
        }

        public async Task<(int totalAssets, int bookedAssets, int soldAssets)> GetAssetStatisticsAsync()
        {
            var totalAssets = await _context.Assets.CountAsync();

            var bookedAssets = await _context.Bookings
                .Where(b => b.Status != "Cancelled" && b.Status != "Confirmed")
                .Select(b => b.AssetId)
                .Distinct()
                .CountAsync();

            var soldStatus = await _context.SystemCodeItems
                .Include(s => s.SystemCode)
                .Where(s => s.SystemCode.Code == "Asset_Status" && s.Code == _appSettings.Asset_Status.SOLD)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var soldAssets = await _context.Assets.CountAsync(a => a.StatusId == soldStatus);

            return (totalAssets, bookedAssets, soldAssets);
        }

    }
}