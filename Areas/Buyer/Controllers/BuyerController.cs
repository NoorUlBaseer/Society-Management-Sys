using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMng.Areas.Buyer.DTOs;
using SocietyMng.Services;
using SocietyMng.Services.Interfaces;
using System.Security.Claims;

namespace SocietyMng.Areas.Buyer.Controllers
{
    [Area("Buyer")]
    [Authorize(Roles = "Buyer")]
    public class BuyerController : Controller
    {
        private readonly IBuyerService _buyerService;

        public BuyerController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = await _buyerService.GetProfileAsync(userId);

            if (profile == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Dashboard");
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Profile model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _buyerService.UpdateProfileAsync(model);
            if (success)
            {
                TempData["Success"] = "Profile updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update profile";
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _buyerService.DeleteAccountAsync(userId);

            if (success)
            {
                return RedirectToAction("Logout", "Account", new { area = "" });
            }
            else
            {
                TempData["Error"] = "Failed to delete account";
                return RedirectToAction("Profile");
            }
        }

        public async Task<IActionResult> MyBookings()
        {
            // Implementation for viewing bookings
            return View();
        }

        public async Task<IActionResult> AssetListings()
        {
            // Implementation for browsing assets
            return View();
        }

        public async Task<IActionResult> MyComplaints()
        {
            // Implementation for complaints
            return View();
        }
    }
}