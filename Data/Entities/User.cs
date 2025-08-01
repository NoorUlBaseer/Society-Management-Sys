namespace SocietyMng.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RoleId { get; set; }
        public SystemCodeItem Role { get; set; }
        public List<Complaint> Complaints { get; set; } = new();
        public List<Asset> UploadedAssets { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }
}