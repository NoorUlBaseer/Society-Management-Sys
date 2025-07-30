using Microsoft.EntityFrameworkCore;
using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Data;
using SocietyMng.Data.Entities;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(AppDbContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // User Management
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.Code != "Admin")
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task BlockUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users .FindAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive; // Toggle instead of just setting to false
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation($"Toggled user {userId} status to {user.IsActive}. Changes saved: {changes}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling user {userId} status");
            throw;
        }
    }

    public async Task DeleteUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _logger.LogInformation($"[DeleteUser] Deleting user {user.FullName} (ID: {user.Id})");

                _context.Users.Remove(user);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation($"[DeleteUser] User deleted. SaveChanges result: {result}");
            }
            else
            {
                _logger.LogWarning($"[DeleteUser] User with ID {userId} not found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[DeleteUser] Error deleting user {userId}");
            throw;
        }
    }


    // Staff Management
    public async Task<List<Staff>> GetAllStaffAsync()
    {
        return await _context.Staff
            .OrderBy(s => s.FullName)
            .ToListAsync();
    }

    public async Task AddStaffAsync(StaffView model)
    {
        var staff = new Staff
        {
            FullName = model.FullName,
            Email = model.Email,
            Position = model.Position,
            Salary = model.Salary,
            HireDate = model.HireDate,
            IsActive = model.IsActive,
            BankAccount = model.BankAccount,
            ContactNumber = model.ContactNumber
        };

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStaffSalaryAsync(SalaryUpdate model)
    {
        var staff = await _context.Staff.FindAsync(model.StaffId);
        if (staff != null)
        {
            staff.Salary = model.NewSalary;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ToggleStaffStatusAsync(int staffId)
    {
        try
        {
            var staff = await _context.Staff.FindAsync(staffId);
            if (staff != null)
            {
                _logger.LogInformation($"[ToggleStaff] Staff {staff.FullName} (ID: {staff.Id}) IsActive before: {staff.IsActive}");

                staff.IsActive = !staff.IsActive;
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation($"[ToggleStaff] Staff {staff.FullName} IsActive after: {staff.IsActive}, SaveChanges result: {result}");
            }
            else
            {
                _logger.LogWarning($"[ToggleStaff] Staff with ID {staffId} not found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ToggleStaff] Error toggling staff status for ID {staffId}");
            throw;
        }
    }


    // Complaint Management
    public async Task<List<Complaint>> GetAllComplaintsAsync()
    {
        return await _context.Complaints
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task ResolveComplaintAsync(int complaintId, string resolution)
    {
        var complaint = await _context.Complaints.FindAsync(complaintId);
        if (complaint != null)
        {
            complaint.Status = ComplaintStatus.Resolved;
            await _context.SaveChangesAsync();
        }
    }

    // Asset Management
    public async Task<List<Asset>> GetAllAssetsAsync()
    {
        return await _context.Assets
            .Include(a => a.RoomCount)
            .Include(a => a.Status)
            .Include(a => a.UploadedByUser)
            .OrderByDescending(a => a.DateUploaded)
            .ToListAsync();
    }
}