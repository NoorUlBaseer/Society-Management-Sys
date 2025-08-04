namespace SocietyMng.Areas.Buyer.DTOs
{
    public class Profile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string NewPassword { get; set; }
        public string RoleId { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
