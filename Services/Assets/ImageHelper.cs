using Microsoft.AspNetCore.Mvc;

namespace SocietyMng.Helpers
{
    public static class ImageHelper
    {
        public static string GetImageUrl(string imagePath, IUrlHelper urlHelper)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return urlHelper.Content("~/images/no-image.png");
            }

            
            if (!imagePath.StartsWith("/"))
            {
                imagePath = "/" + imagePath;
            }

            return urlHelper.Content($"~{imagePath}");
        }

        public static bool ImageExists(string imagePath, IWebHostEnvironment webHostEnvironment)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fullPath = Path.Combine(webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            return File.Exists(fullPath);
        }
    }
}