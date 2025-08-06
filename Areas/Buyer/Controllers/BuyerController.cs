using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMng.Areas.Buyer.DTOs;
using SocietyMng.Data;
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
                return RedirectToAction("Landing", "Auth", new { area = "" });
            }
            else
            {
                TempData["Error"] = "Failed to delete account";
                return RedirectToAction("Profile");
            }
        }

        public async Task<IActionResult> AssetListing()
        {
            var assets = await _buyerService.GetAllAssetsAsync();
            return View(assets);
        }

        [HttpGet]
        public async Task<IActionResult> AssetDetails(int id)
        {
            var asset = await _buyerService.GetAssetByIdAsync(id);
            if (asset == null)
            {
                TempData["Error"] = "Asset not found";
                return RedirectToAction("AssetListing");
            }

            return View(asset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAsset(int assetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _buyerService.BookAssetAsync(userId, assetId);

            if (result.Success)
            {
                TempData["Success"] = "Asset booked successfully!";
                return RedirectToAction("MyBookings");
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction("AssetListing");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var bookings = await _buyerService.GetUserBookingsAsync(userId);
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _buyerService.CancelBookingAsync(userId, bookingId);

            if (result.Success)
            {
                TempData["Success"] = "Booking cancelled successfully!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("MyBookings");
        }

        //[HttpGet]
        //public async Task<IActionResult> MyComplaints()
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    var complaints = await _buyerService.GetUserComplaintsAsync(userId);
        //    return View(complaints);
        //}

        //[HttpGet]
        //public async Task<IActionResult> CreateComplaint()
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //    // Get user's bookings to allow complaint filing
        //    var bookings = await _buyerService.GetUserBookingsAsync(userId);
        //    ViewBag.Bookings = bookings;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateComplaint(int? assetId, int? bookingId, string subject, string description)
        //{
        //    if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description))
        //    {
        //        TempData["Error"] = "Subject and description are required";
        //        return RedirectToAction("CreateComplaint");
        //    }

        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //    try
        //    {
        //        var complaint = new Complaint
        //        {
        //            UserId = userId,
        //            AssetId = assetId,
        //            BookingId = bookingId,
        //            Description = description,
                   
        //            CreatedAt = DateTime.Now
        //        };

        //        _context.Complaints.Add(complaint);
        //        await _context.SaveChangesAsync();

        //        TempData["Success"] = "Complaint submitted successfully!";
        //        return RedirectToAction("MyComplaints");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Failed to submit complaint. Please try again.";
        //        return RedirectToAction("CreateComplaint");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> BookingDetails(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var bookings = await _buyerService.GetUserBookingsAsync(userId);
            var booking = bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found";
                return RedirectToAction("MyBookings");
            }

            return View(booking);
        }

    }
}