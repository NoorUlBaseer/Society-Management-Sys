namespace SocietyMng.Data.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum ComplaintStatus
    {
        Pending,
        Resolved,
        Rejected
    }
}