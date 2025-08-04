using Microsoft.EntityFrameworkCore;
using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Data;
using SocietyMng.Data.Entities;
using SocietyMng.Configurations;
using Microsoft.Extensions.Options;

namespace SocietyMng.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminService> _logger;
        private readonly AppSettings _appSettings;

        public AdminService(AppDbContext context, ILogger<AdminService> logger, IOptions<AppSettings> appSets)
        {
            _context = context;
            _logger = logger;
            _appSettings = appSets.Value;
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
                var asset = new Asset
                {
                    Description = model.Description,
                    Address = model.Address,
                    PlotNumber = model.PlotNumber,
                    ImagePath = model.ImagePath, // Make sure this is uncommented
                    Price = model.Price,
                    BlockId = model.BlockId,
                    PropertyTypeId = model.PropertyTypeId,
                    StatusId = model.StatusId,
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




        //public async Task<Asset> UpdateAssetAsync(int id, AssetUpdateDto model)
        //{
        //    _logger.LogDebug("Updating asset ID: {AssetId}", id);

        //    var asset = await _context.Assets.FindAsync(id);
        //    if (asset == null)
        //    {
        //        _logger.LogWarning("Asset with ID: {AssetId} not found for update", id);
        //        throw new KeyNotFoundException($"Asset with ID {id} not found");
        //    }

        //    asset.Description = model.Description ?? asset.Description;
        //    asset.Address = model.Address ?? asset.Address;
        //    asset.PlotNumber = model.PlotNumber ?? asset.PlotNumber;
        //    asset.ImagePath = model.ImagePath ?? asset.ImagePath;
        //    asset.Price = model.Price ?? asset.Price;
        //    asset.BlockId = model.BlockId ?? asset.BlockId;
        //    asset.PropertyTypeId = model.PropertyTypeId ?? asset.PropertyTypeId;
        //    asset.StatusId = model.StatusId ?? asset.StatusId;

        //    await _context.SaveChangesAsync();

        //    _logger.LogInformation("Successfully updated asset ID: {AssetId}", id);
        //    return asset;
        //}

        //public async Task DeleteAssetAsync(int id)
        //{
        //    _logger.LogDebug("Deleting asset ID: {AssetId}", id);

        //    var asset = await _context.Assets.FindAsync(id);
        //    if (asset == null)
        //    {
        //        _logger.LogWarning("Asset with ID: {AssetId} not found for deletion", id);
        //        throw new KeyNotFoundException($"Asset with ID {id} not found");
        //    }

        //    _context.Assets.Remove(asset);
        //    await _context.SaveChangesAsync();

        //    _logger.LogInformation("Successfully deleted asset ID: {AssetId}", id);
        //}

        //    // Staff Management
        //    public async Task<List<Staff>> GetAllStaffAsync()
        //    {
        //        _logger.LogDebug("Fetching all staff members from database");
        //        var staff = await _context.Staff.ToListAsync();
        //        _logger.LogInformation("Retrieved {StaffCount} staff members", staff.Count);
        //        return staff;
        //    }

        //    public async Task AddStaffAsync(StaffView model)
        //    {
        //        _logger.LogDebug("Adding new staff member: {StaffName}", model.FullName);
        //        var staff = new Staff
        //        {
        //            FullName = model.FullName,
        //            Email = model.Email,
        //            ContactNumber = model.ContactNumber,
        //            Position = model.Position,
        //            Salary = model.Salary,
        //            HireDate = model.HireDate,
        //            IsActive = model.IsActive,
        //            BankAccount = model.BankAccount
        //        };

        //        await _context.Staff.AddAsync(staff);
        //        await _context.SaveChangesAsync();
        //        _logger.LogInformation("Successfully added new staff member ID: {StaffId}, Name: {StaffName}", staff.Id, staff.FullName);
        //    }

        //    public async Task UpdateStaffSalaryAsync(SalaryUpdate model)
        //    {
        //        if (model == null || model.StaffId <= 0 || model.NewSalary <= 0)
        //        {
        //            _logger.LogWarning("Invalid salary update model received");
        //            throw new ArgumentException("Invalid salary update parameters");
        //        }

        //        var staff = await _context.Staff.FindAsync(model.StaffId);
        //        if (staff == null)
        //        {
        //            _logger.LogWarning("Staff ID: {StaffId} not found for salary update", model.StaffId);
        //            throw new KeyNotFoundException($"Staff with ID {model.StaffId} not found");
        //        }

        //        staff.Salary = model.NewSalary;
        //        await _context.SaveChangesAsync();
        //        _logger.LogInformation("Salary updated for {StaffName} to {NewSalary}",
        //            staff.FullName, model.NewSalary);
        //    }

        //    public async Task ToggleStaffStatusAsync(int staffId)
        //    {
        //        _logger.LogDebug("Attempting to toggle status for staff ID: {StaffId}", staffId);
        //        var staff = await _context.Staff.FindAsync(staffId);
        //        if (staff != null)
        //        {
        //            staff.IsActive = !staff.IsActive;
        //            await _context.SaveChangesAsync();
        //            _logger.LogInformation("Staff ID: {StaffId} status changed to {Status}", staffId, staff.IsActive ? "Active" : "Inactive");
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Staff ID: {StaffId} not found for status toggle", staffId);
        //        }
        //    }

        //    // Complaint Management
        //    public async Task<List<Complaint>> GetAllComplaintsAsync()
        //    {
        //        _logger.LogDebug("Fetching all complaints from database");
        //        var complaints = await _context.Complaints
        //            .Include(c => c.User)
        //            .ToListAsync();
        //        _logger.LogInformation("Retrieved {ComplaintCount} complaints", complaints.Count);
        //        return complaints;
        //    }
    }
}