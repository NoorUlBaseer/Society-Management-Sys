using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Areas.Admin.DTOs
{
    public class AssetCreateView
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(1000, ErrorMessage = "Address cannot exceed 1000 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plot number is required")]
        [StringLength(50, ErrorMessage = "Plot number cannot exceed 50 characters")]
        public string PlotNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Block is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid block")]
        public int BlockId { get; set; }

        [Required(ErrorMessage = "Property type is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid property type")]
        public int PropertyTypeId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid status")]
        public int StatusId { get; set; }

        // This property is set automatically during file upload - NOT required for validation
        public string? ImagePath { get; set; }
    }

public class AssetUpdateView
    {
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PlotNumber { get; set; }
        public decimal? Price { get; set; }
        public int? BlockId { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? StatusId { get; set; }
    }
}