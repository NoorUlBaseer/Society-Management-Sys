using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietyMng.Data;
using SocietyMng.Data.Entities;
using SocietyMng.Models.Auth;
using System.Security.Claims;

namespace SocietyMng.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? backURL = null)
        {
            ViewData["ReturnUrl"] = backURL;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string? backURL = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid Email or password!");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been blocked by the administrator. Please contact support.");
                return View(model);
            }
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Role, user.Role.Code)
               
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                authProperties);

            //id admin-> admin dashboard
            if (user.Role.Code == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
            }

            return RedirectToLocal(backURL);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered");
                return View(model);
            }

            var roleItem = await _context.SystemCodeItems
                .Include(sci => sci.SystemCode)
                .Where(sci => sci.Code == model.SelectedRole)
                .Where(sci => sci.SystemCode.Code == "User_Role")
                .FirstOrDefaultAsync();

            if (roleItem == null)
            {
                ModelState.AddModelError("SelectedRole", "Invalid role selected");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender ?? "Unknown",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RoleId = roleItem.Id
            };

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await Login(new LoginModel
                {
                    Email = model.Email,
                    Password = model.Password,
                });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception exep)
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Login", "Auth");
        }

        //helper functions
        private static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        private static bool VerifyPassword(string enteredPassword, string storedHash) =>
            BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);

        private IActionResult RedirectToLocal(string backURL) =>
            Url.IsLocalUrl(backURL) ? Redirect(backURL) : RedirectToAction("Index", "Home");
    }
}