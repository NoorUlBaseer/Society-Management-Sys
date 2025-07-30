using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMng.Areas.Admin.DTOs;

[Area("Admin")]
[Authorize(Roles = "Admin")]

public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    // User Management
    public async Task<IActionResult> ManageUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockUser(int userId)
    {
        try
        {
            await _adminService.BlockUserAsync(userId);
            TempData["Message"] = "User blocked successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking user");
            TempData["Error"] = "Failed to block user";
        }
        return RedirectToAction(nameof(ManageUsers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> DeleteUser(int userId)
    {
        await _adminService.DeleteUserAsync(userId);
        TempData["Message"] = "User deleted successfully";
        return RedirectToAction(nameof(ManageUsers));
    }

    // Staff Management
    public async Task<IActionResult> ManageStaff()
    {
        var staff = await _adminService.GetAllStaffAsync();
        return View(staff);
    }

    [HttpGet]
    public IActionResult AddStaff()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddStaff(StaffView model)
    {
        if (!ModelState.IsValid) return View(model);

        await _adminService.AddStaffAsync(model);
        TempData["Message"] = "Staff added successfully";
        return RedirectToAction(nameof(ManageStaff));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSalary(SalaryUpdate model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid salary update request";
            return RedirectToAction(nameof(ManageStaff));
        }

        await _adminService.UpdateStaffSalaryAsync(model);
        TempData["Message"] = "Salary updated successfully";
        return RedirectToAction(nameof(ManageStaff));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStaffStatus(int staffId)
    {
        await _adminService.ToggleStaffStatusAsync(staffId);
        TempData["Message"] = "Staff status updated";
        return RedirectToAction(nameof(ManageStaff));
    }

    // Complaint Management
    public async Task<IActionResult> ManageComplaints()
    {
        var complaints = await _adminService.GetAllComplaintsAsync();
        return View(complaints);
    }

    [HttpPost]
    public async Task<IActionResult> ResolveComplaint(int complaintId)
    {
        await _adminService.ResolveComplaintAsync(complaintId, "Resolved by admin");
        TempData["Message"] = "Complaint resolved successfully";
        return RedirectToAction(nameof(ManageComplaints));
    }

    // Asset Management
    public async Task<IActionResult> ManageAssets()
    {
        var assets = await _adminService.GetAllAssetsAsync();
        return View(assets);
    }

}