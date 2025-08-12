using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMng.Areas.Buyer.ModelView;
using SocietyMng.Configurations;
using SocietyMng.Data;
using SocietyMng.Data.Entities;
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
        private readonly ILogger<BuyerController> _logger;

        public BuyerController(IBuyerService buyerService, AppDbContext context, ILogger<BuyerController> logger)
        {
            _buyerService = buyerService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            return View();
        }
        public async Task<IActionResult> AboutUs()
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
                TempData["Error"] = "Failed to update profile. Please check your inputs (email format, phone number format, or ensure new password is different from current password).\";";
            }

            return RedirectToAction("Profile");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(string password, string confirmText)
        {
            if (string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Password is required to delete account";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrEmpty(confirmText) || !confirmText.Equals("Confirm", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Please type 'Confirm' to delete your account";
                return RedirectToAction("Profile");
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _logger.LogInformation("User {UserId} attempting to delete account", userId);

                var success = await _buyerService.DeleteAccountAsync(userId, password);

                if (success)
                {
                    _logger.LogInformation("Account successfully deleted for user {UserId}", userId);
                    TempData["Success"] = "Account deleted successfully";
                    return RedirectToAction("Landing", "Auth", new { area = "" });
                }
                else
                {
                    _logger.LogWarning("Failed to delete account for user {UserId} - likely invalid password", userId);
                    TempData["Error"] = "Failed to delete account. Please check your password and try again.";
                    return RedirectToAction("Profile");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting account");
                TempData["Error"] = "An unexpected error occurred while deleting your account. Please try again.";
                return RedirectToAction("Profile");
            }
        }

        public async Task<IActionResult> Index()
        {
            var assets = await _buyerService.GetAllAssetsAsync();
            var priceRange = await _buyerService.GetPriceRangeAsync();

            ViewBag.MinPrice = priceRange.minPrice;
            ViewBag.MaxPrice = priceRange.maxPrice;

            return View(assets);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var asset = await _buyerService.GetAssetByIdAsync(id);
                return View(asset);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int id)
        {
            _logger.LogInformation("Book method called with id: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid asset id: {Id}", id);
                return BadRequest();
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _logger.LogInformation("Booking asset {AssetId} for user {UserId}", id, userId);

                var result = await _buyerService.BookAssetAsync(userId, id);

                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index");
                }

                TempData["Success"] = result.Message;
                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking asset {AssetId}", id);
                TempData["Error"] = "An error occurred while booking the asset";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId)
        
        {
            _logger.LogInformation("Cancel method called with bookingId: {BookingId}", bookingId);

            if (bookingId <= 0)
            {
                _logger.LogWarning("Invalid booking ID: {BookingId}", bookingId);
                TempData["Error"] = "Invalid booking ID provided.";
                return RedirectToAction(nameof(MyBookings));
            }
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                _logger.LogError("Could not parse valid user ID from claims");
                TempData["Error"] = "Authentication error. Please log in again.";
                return RedirectToAction("Landing", "Auth", new { area = "" });
            }

            try
            {
                _logger.LogInformation("Attempting to cancel booking {BookingId} for user {UserId}", bookingId, userId);

                var result = await _buyerService.CancelBookingAsync(userId, bookingId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to cancel booking {BookingId}: {Message}", bookingId, result.Message);
                    TempData["Error"] = result.Message;
                }
                else
                {
                    _logger.LogInformation("Successfully cancelled booking {BookingId} for user {UserId}", bookingId, userId);
                    TempData["Success"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while cancelling booking {BookingId} for user {UserId}", bookingId, userId);
                TempData["Error"] = "An unexpected error occurred while cancelling the booking. Please try again.";
            }

            return RedirectToAction(nameof(MyBookings));
        }
        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var bookings = await _buyerService.GetUserBookingsAsync(userId);
            return View(bookings);
        }
    }
}