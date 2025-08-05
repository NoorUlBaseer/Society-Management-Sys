using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Configurations;
using SocietyMng.Data;
using SocietyMng.Data.Entities;

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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AssetController(IAdminService adminService, AppDbContext context, IOptions<AppSettings> appSet,
            ILogger<AssetController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _adminService = adminService;
            _context = context;
            _appSettings = appSet.Value;
            _logger = logger;
            //for to wwwroot -> public asset img folder
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var assets= await _adminService.GetAllAssetsAsync();
            return View(assets);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.Block)
                .Include (a => a.PropertyType)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
            {
                TempData["Error"] = "Asset not found!";
                return RedirectToAction("Index");
            }

            return View(asset);
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
                ModelState.Remove("ImagePath");

               
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    _logger.LogWarning("No image file provided");
                    ModelState.AddModelError("ImageFile", "Please select an image file.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid for Create Asset");

                    foreach (var error in ModelState)
                    {
                        _logger.LogWarning("ModelState Error - Key: {Key}, Errors: {Errors}",
                            error.Key, string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                    }

                    await PopulateLookupsAsync();
                    ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                    ViewBag.ErrorMessage = "Please correct the validation errors below.";
                    return View(model);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    _logger.LogInformation("Processing image upload: {FileName}, Size: {Size} bytes",
                        ImageFile.FileName, ImageFile.Length);

                    // img extension check
                    var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                    if (!_appSettings.FileUploadPath.AllowedExtensions.Contains(ext))
                    {
                        _logger.LogWarning("Invalid file extension: {Extension}", ext);
                        ModelState.AddModelError("ImageFile", $"Invalid image format. Allowed formats: {string.Join(", ", _appSettings.FileUploadPath.AllowedExtensions)}");
                        await PopulateLookupsAsync();
                        ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                        return View(model);
                    }

                    // img size check
                    var maxBytes = _appSettings.FileUploadPath.MaxFileSizeMB * 1024 * 1024;
                    if (ImageFile.Length > maxBytes)
                    {
                        _logger.LogWarning("File size {Size} exceeds maximum {MaxSize}", ImageFile.Length, maxBytes);
                        ModelState.AddModelError("ImageFile", $"File size exceeds {_appSettings.FileUploadPath.MaxFileSizeMB} MB.");
                        await PopulateLookupsAsync();
                        ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                        return View(model);
                    }

                    //iwebenv config -> for wwwroot
                    var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, _appSettings.FileUploadPath.AssetImages);

                    _logger.LogInformation("Upload directory path: {Directory}", uploadsPath);
                    _logger.LogInformation("WebRoot path: {WebRoot}", _webHostEnvironment.WebRootPath);
                    _logger.LogInformation("Configured asset images path: {AssetPath}", _appSettings.FileUploadPath.AssetImages);

                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                        _logger.LogInformation("Created upload directory: {Directory}", uploadsPath);
                    }
                    else
                    {
                        _logger.LogInformation("Upload directory already exists: {Directory}", uploadsPath);
                    }

                    // generate img path  by timestamp
                    var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var fileName = $"{timeStamp}_{Guid.NewGuid()}{ext}";
                    var fullFilePath = Path.Combine(uploadsPath, fileName);

                    _logger.LogInformation("Saving file to: {FilePath}", fullFilePath);

                   
                    using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }

                    if (System.IO.File.Exists(fullFilePath))
                    {
                        var fileInfo = new FileInfo(fullFilePath);
                        _logger.LogInformation("File saved successfully. Size: {Size} bytes, Path: {Path}",
                            fileInfo.Length, fullFilePath);
                    }
                    else
                    {
                        _logger.LogError("File was not saved successfully: {Path}", fullFilePath);
                        throw new InvalidOperationException("Failed to save uploaded file");
                    }

                    //relative wwwroot folder
                    var webRelativePath = $"/{_appSettings.FileUploadPath.AssetImages.Replace("\\", "/")}/{fileName}";
                    model.ImagePath = webRelativePath;

                    _logger.LogInformation("Image saved with web path: {WebPath}", webRelativePath);
                }

                // foreign key checks
                var blockExists = await _context.SystemCodeItems.AnyAsync(x => x.Id == model.BlockId);
                var propertyTypeExists = await _context.SystemCodeItems.AnyAsync(x => x.Id == model.PropertyTypeId);

                if (!blockExists || !propertyTypeExists)
                {
                    if (!blockExists) ModelState.AddModelError("BlockId", "Selected block is invalid.");
                    if (!propertyTypeExists) ModelState.AddModelError("PropertyTypeId", "Selected property type is invalid.");

                    await PopulateLookupsAsync();
                    ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath.MaxFileSizeMB;
                    return View(model);
                }
              
                await _adminService.AddAssetAsync(model);
                _logger.LogInformation("Asset created successfully with ImagePath: {ImagePath}", model.ImagePath);

                TempData["SuccessMessage"] = "Asset created successfully!";
                return RedirectToAction("Index", "Asset", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                //ViewBag ->to tell the frontend about file size
                _logger.LogError(ex, "Error creating asset");
                ViewBag.ErrorMessage = $"An error occurred while creating the asset: {ex.Message}";
                await PopulateLookupsAsync();
                ViewBag.MaxFileSizeMB = _appSettings.FileUploadPath?.MaxFileSizeMB ?? 20;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _context.Assets.FindAsync(id);


            if (asset == null)
            {
                TempData["Error"] = "Asset not found!";
                return RedirectToAction("Index");
            }

            var model = new AssetUpdateView
            {
                Id = asset.Id,
                Description = asset.Description,
                Address = asset.Address,
                PlotNumber = asset.PlotNumber,
                Price= asset.Price,
                BlockId= asset.BlockId,
                PropertyTypeId = asset.PropertyTypeId,
                StatusId = asset.StatusId,
                ImagePath = asset.ImagePath,
            };
            ViewBag.ExistingImageUrl = asset.ImagePath;
            await PopulateLookupsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AssetUpdateView model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateLookupsAsync();
                return View(model);
            }
            var success = await _adminService.UpdateAssetAsync(id, model);
            if (success== null)
            {
                TempData["Error"] = "Failed to update asset.";
                return View(model);
            }

            TempData["Success"] = "Asset updated successfully!";
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            
            await _adminService.DeleteAssetAsync(id);
            TempData["SuccessMessage"] = "Asset deleted successfully!";
            _logger.LogWarning("Successfully deleted Asset ID: {id}", id);
            return RedirectToAction(nameof(Index));
        }

        //to  extract dropdown val from the db ->to avoid hardcoding
        private async Task PopulateLookupsAsync()
        {
            try
            {
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