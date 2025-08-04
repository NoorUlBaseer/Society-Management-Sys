using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Configurations;
using SocietyMng.Data;

namespace SocietyMng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AssetController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly ILogger<AssetController> _logger;

        public AssetController(IAdminService adminService, AppDbContext context, IOptions<AppSettings> appSet, ILogger<AssetController> logger)
        {
            _adminService = adminService;
            _context = context;
            _appSettings = appSet.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                _logger.LogDebug("Loading Create Asset form");
                await PopulateLookupsAsync();
                ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create Asset form");
                TempData["Error"] = "Error loading the form. Please try again.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetCreateView model, IFormFile ImageFile)
        {
            try
            {
                _logger.LogDebug("Processing Create Asset POST request");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid for Create Asset");
                    await PopulateLookupsAsync();
                    ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                    return View(model);
                }

                if (ImageFile != null)
                {
                    _logger.LogDebug("Processing image upload: {FileName}", ImageFile.FileName);

                    // Validate extension
                    var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                    if (!_appSettings.FileUploadPath.AllowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("ImageFile", "Invalid image format.");
                        await PopulateLookupsAsync();
                        ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                        return View(model);
                    }

                    // Validate size
                    var maxBytes = _appSettings.FileUploadPath.MaxFileSizeMB * 1024 * 1024;
                    if (ImageFile.Length > maxBytes)
                    {
                        ModelState.AddModelError("ImageFile", $"File size exceeds {_appSettings.FileUploadPath.MaxFileSizeMB} MB.");
                        await PopulateLookupsAsync();
                        ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                        return View(model);
                    }

                    // Save file
                    var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _appSettings.FileUploadPath.AssetImages);
                    if (!Directory.Exists(uploadsRoot))
                        Directory.CreateDirectory(uploadsRoot);

                    var fileName = Guid.NewGuid() + ext;
                    var filePath = Path.Combine(uploadsRoot, fileName);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fs);
                    }

                    // Store relative path
                    model.ImagePath = Path.Combine(_appSettings.FileUploadPath.AssetImages, fileName).Replace("\\", "/");
                }

                // Save asset to database
                await _adminService.AddAssetAsync(model);
                _logger.LogInformation("Asset created successfully");

                // Redirect to dashboard after successful creation
                TempData["SuccessMessage"] = "Asset created successfully!";
                return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset");
                ViewBag.ErrorMessage = "An error occurred while creating the asset. Please try again.";
                await PopulateLookupsAsync();
                ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogDebug("Loading Assets Index");
                var assets = await _context.Assets
                    .Include(a => a.Block)
                    .Include(a => a.PropertyType)
                    .Include(a => a.Status)
                    .ToListAsync();
                return View(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assets");
                TempData["Error"] = "Error loading assets. Please try again.";
                return View(new List<SocietyMng.Data.Entities.Asset>());
            }
        }

        private async Task PopulateLookupsAsync()
        {
            try
            {
                // Fix: Get lookup data from SystemCodeItems with Include
                ViewBag.Blocks = await _context.SystemCodeItems
                    .Include(x => x.SystemCode)
                    .Where(x => x.SystemCode.Code == "Block" && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                ViewBag.PropertyTypes = await _context.SystemCodeItems
                    .Include(x => x.SystemCode)
                    .Where(x => x.SystemCode.Code == "Property_Type" && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                ViewBag.Statuses = await _context.SystemCodeItems
                    .Include(x => x.SystemCode)
                    .Where(x => x.SystemCode.Code == "Asset_Status" && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                _logger.LogDebug("Populated lookups - Blocks: {BlockCount}, PropertyTypes: {PropertyTypeCount}, Statuses: {StatusCount}",
                    ((List<SocietyMng.Data.Entities.SystemCodeItem>)ViewBag.Blocks).Count,
                    ((List<SocietyMng.Data.Entities.SystemCodeItem>)ViewBag.PropertyTypes).Count,
                    ((List<SocietyMng.Data.Entities.SystemCodeItem>)ViewBag.Statuses).Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating lookups");
                ViewBag.Blocks = new List<SocietyMng.Data.Entities.SystemCodeItem>();
                ViewBag.PropertyTypes = new List<SocietyMng.Data.Entities.SystemCodeItem>();
                ViewBag.Statuses = new List<SocietyMng.Data.Entities.SystemCodeItem>();
            }
        }
    }
}