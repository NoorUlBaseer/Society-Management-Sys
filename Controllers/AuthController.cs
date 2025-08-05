using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocietyMng.Configurations;
using SocietyMng.Data;
using SocietyMng.Data.Entities;
using SocietyMng.Models.Auth;
using System.Security.Claims;

namespace SocietyMng.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;

        public AuthController(AppDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public IActionResult Landing()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string? backURL = null)
        {
            // HARDCODED ADMIN LOGIC JUST FOR TESTING PURPOSES
            if (model.Email == "admin@local.dev" && model.Password == "admin123")
            {
                var adminClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, "999"), // fake ID
                    new(ClaimTypes.Email, model.Email),
                    new(ClaimTypes.Name, "Local Admin"),
                    new(ClaimTypes.Role, "Admin") // this must match _appSettings.User_Role.Admin
                };

                var adminAuthProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    adminAuthProperties
                );

                return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
            }

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
            if (user.Role.Code == _appSettings.User_Role.Admin)
            {
                return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
            }

            else if (user.Role.Code == _appSettings.User_Role.Buyer)
            {
                return RedirectToAction("Dashboard", "Buyer", new { area = "Buyer" });
            }

            return RedirectToLocal(backURL);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            var roleItem = await _context.SystemCodeItems
                .Include(sci => sci.SystemCode)
                .FirstOrDefaultAsync(sci =>
                    sci.Code == model.SelectedRole &&
                    sci.SystemCode.Code == "User_Role");

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
                Gender = model.Gender,
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
                    Password = model.Password
                });
                if (model.SelectedRole == _appSettings.User_Role.Buyer)
                    return RedirectToAction("Dashboard", "Buyer", new { area = "Buyer" });

                return RedirectToAction("Index", "Home");
            }
            catch
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

            // More explicit redirect with controller specified
            return RedirectToAction("Landing", "Auth");
        }

        private static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        private static bool VerifyPassword(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);

        private IActionResult RedirectToLocal(string returnUrl) =>
            Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
    }
}