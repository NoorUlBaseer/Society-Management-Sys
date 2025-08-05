using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMng.Areas.Buyer.DTOs;
using SocietyMng.Data;
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
        private readonly AppDbContext _context; 

        public BuyerController(IBuyerService buyerService, AppDbContext context)
        {
            _buyerService = buyerService;
            _context = context;
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
        public async Task<IActionResult> Profile(Profile model, string newPassword, string confirmNewPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmNewPassword)
                {
                    TempData["Error"] = "New password and confirmation password do not match";
                    return View(model);
                }
            }

            var profileToUpdate = new Profile
            {
                Id = model.Id,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                NewPassword = model.NewPassword
            };

            var success = await _buyerService.UpdateProfileAsync(profileToUpdate);
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
                return RedirectToAction("Landing","Auth", new { area = "" });
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

        public async Task<IActionResult> AssetListing()
        {
            var assets = await _buyerService.GetAllAssetsAsync();
            return View(assets);
        }


        public async Task<IActionResult> MyComplaints()
        {
            // Implementation for complaints
            return View();
        }
    }
}

        

//        public async Task<IActionResult> MyComplaints()
//        {
//            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var complaints = await _buyerService.GetUserComplaintsAsync(userId);
//            return View(complaints);
//        }
//    }
//}