namespace SocietyMng.Areas.Admin.DTOs
{
    public class AssetView
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PlotNumber { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string Block { get; set; }
        public string PropertyType { get; set; }
        public string Status { get; set; }
        public IFormFile ImageFile { get; set; } 
    }
}