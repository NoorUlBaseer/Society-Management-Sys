namespace SocietyMng.Data.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ComplaintStatus Status { get; set; }
        public string Resolution { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum ComplaintStatus
    {
        Pending,
        Resolved,
        Rejected
    }

}
