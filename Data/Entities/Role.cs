namespace SocietyMng.Data.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Admin,User -> Resident, Buyer
        public List<UserRole> UserRoles { get; set; } = new();
    }

    // Joined table
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}