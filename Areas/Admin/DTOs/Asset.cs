namespace SocietyMng.Areas.Admin.DTOs
{
    public class AssetCreateView
    {
        public string Description { get; set; }
        public string Address { get; set; }
        public string PlotNumber { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int BlockId { get; set; }
        public int PropertyTypeId { get; set; }
        public int StatusId { get; set; }
    }

    public class AssetUpdateView
    {
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PlotNumber { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Price { get; set; }
        public int? BlockId { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? StatusId { get; set; }
    }
}