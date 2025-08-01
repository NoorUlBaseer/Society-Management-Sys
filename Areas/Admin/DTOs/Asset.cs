namespace SocietyMng.Areas.Admin.DTOs
{
    public class AssetView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Today;
        public bool IsActive { get; set; } = true;
    }
}
