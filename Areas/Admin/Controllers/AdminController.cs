using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Data.Entities;
using Microsoft.Extensions.Logging;

namespace SocietyMng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
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
            _logger.LogInformation("Admin dashboard accessed");
            return View();
        }

        #region User Management

        public async Task<IActionResult> ManageUsers()
        {
            _logger.LogDebug("Fetching all users for management");
            var users = await _adminService.GetAllUsersAsync();
            _logger.LogInformation("Retrieved {UserCount} users for management", users.Count);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            _logger.LogInformation("Attempting to toggle status for user ID: {UserId}", userId);
            await _adminService.BlockUserAsync(userId);
            _logger.LogInformation("Successfully toggled status for user ID: {UserId}", userId);
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            _logger.LogWarning("Attempting to delete user ID: {UserId}", userId);
            await _adminService.DeleteUserAsync(userId);
            _logger.LogWarning("Successfully deleted user ID: {UserId}", userId);
            return RedirectToAction(nameof(ManageUsers));
        }

        #endregion

        #region Asset Management Routes
        public IActionResult ManageAssets()
        {
            return RedirectToAction("Index", "Asset", new { area = "Admin" });
        }

        #endregion


        //    #region Staff Management

        //    public async Task<IActionResult> ManageStaff()
        //    {
        //        _logger.LogDebug("Fetching all staff members for management");
        //        var staff = await _adminService.GetAllStaffAsync();
        //        _logger.LogInformation("Retrieved {StaffCount} staff members", staff.Count);
        //        return View(staff);
        //    }

        //    [HttpGet]
        //    public IActionResult AddStaff()
        //    {
        //        _logger.LogDebug("AddStaff form requested");
        //        return View();
        //    }

        //    [HttpPost]
        //    public async Task<IActionResult> AddStaff(StaffView model)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogWarning("AddStaff form submitted with invalid model state");
        //            return View(model);
        //        }

        //        _logger.LogInformation("Attempting to add new staff member: {StaffName}", model.FullName);
        //        await _adminService.AddStaffAsync(model);
        //        _logger.LogInformation("Successfully added new staff member: {StaffName}", model.FullName);
        //        return RedirectToAction(nameof(ManageStaff));
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> UpdateSalary(int staffId, decimal newSalary)
        //    {
        //        if (staffId <= 0 || newSalary <= 0)
        //        {
        //            _logger.LogWarning("Invalid salary update parameters");
        //            return RedirectToAction(nameof(ManageStaff));
        //        }

        //        _logger.LogInformation("Updating salary for staff ID: {StaffId}. New salary: {NewSalary}",
        //            staffId, newSalary);

        //        await _adminService.UpdateStaffSalaryAsync(new SalaryUpdate
        //        {
        //            StaffId = staffId,
        //            NewSalary = newSalary
        //        });

        //        return RedirectToAction(nameof(ManageStaff));
        //    }

        //    [HttpPost]
        //    public async Task<IActionResult> ToggleStaffStatus(int staffId)
        //    {
        //        _logger.LogInformation("Toggling status for staff ID: {StaffId}", staffId);
        //        await _adminService.ToggleStaffStatusAsync(staffId);
        //        _logger.LogInformation("Successfully toggled status for staff ID: {StaffId}", staffId);
        //        return RedirectToAction(nameof(ManageStaff));
        //    }

        //    #endregion

        //    #region Complaint Management

        //    public async Task<IActionResult> ManageComplaints()
        //    {
        //        _logger.LogDebug("Fetching all complaints for management");
        //        var complaints = await _adminService.GetAllComplaintsAsync();
        //        _logger.LogInformation("Retrieved {ComplaintCount} complaints", complaints.Count);
        //        return View(complaints);
        //    }

        //    #endregion
    }
}
