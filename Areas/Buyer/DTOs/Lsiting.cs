using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Areas.Admin.DTOs
{
    public class AssetListingView
    {
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PlotNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int BlockId { get; set; }
        public int PropertyTypeId { get; set; }
        public int StatusId { get; set; }
        public string? ImagePath { get; set; }
    }
}