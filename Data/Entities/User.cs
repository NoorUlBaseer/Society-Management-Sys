namespace SocietyMng.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RoleId { get; set; }
        public SystemCodeItem Role { get; set; }
        public List<Complaint> Complaints { get; set; } = new();
    }
}