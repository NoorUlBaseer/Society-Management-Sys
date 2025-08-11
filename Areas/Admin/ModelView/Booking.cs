using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Areas.Admin.ModelView
{
    public class BookedAssetView
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }

        public int AssetId { get; set; }
        public string AssetDescription { get; set; }
        public string AssetBlock { get; set; }
        public string AssetType { get; set; }
        public decimal AssetPrice { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
    }
}