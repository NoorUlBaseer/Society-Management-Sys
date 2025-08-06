namespace SocietyMng.Models
{
    public class BookingViewModel
    {
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
