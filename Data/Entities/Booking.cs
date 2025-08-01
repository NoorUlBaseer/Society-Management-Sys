namespace SocietyMng.Data.Entities
{
    // Booking.cs
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public DateTime BookingDate { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Cancelled
        public DateTime CreatedAt { get; set; }

        public List<Complaint> Complaints { get; set; } = new();
    }
}
