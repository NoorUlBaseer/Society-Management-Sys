//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//[Area("Admin")]
//[Authorize(Roles = "Admin")]
//public class AdminController : Controller
//{
//    private readonly IAdminService _adminService;
//    private readonly ILogger<AdminController> _logger;

//    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
//    {
//        _adminService = adminService;
//        _logger = logger;
//    }

//    public async Task<IActionResult> Dashboard()
//    {
//        return View();
//    }

//    public async Task<IActionResult> ManageUsers()
//    {
//        var users = await _adminService.GetAllUsersAsync();
//        return View(users);
//    }

//    [HttpPost]
//    public async Task<IActionResult> BlockUser(int userId)
//    {
//        await _adminService.BlockUserAsync(userId);
//        return RedirectToAction(nameof(ManageUsers));
//    }

//    public async Task<IActionResult> GenerateDocument(string type, int id)
//    {
//        var document = await _adminService.GeneratePropertyDocument(type, id);
//        return Content(document);
//    }

//    [HttpPost]
//    public async Task<IActionResult> UpdateSalary(int staffId, decimal salary)
//    {
//        await _adminService.UpdateStaffSalary(staffId, salary);
//        return RedirectToAction(nameof(StaffManagement));
//    }

   
//}