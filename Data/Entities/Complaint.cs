namespace SocietyMng.Data.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? AssetId { get; set; }
        public Asset Asset { get; set; }
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }
    }

    public enum ComplaintStatus
    {
        Pending,
        Resolved,
        Rejected
    }
}