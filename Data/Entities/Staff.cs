namespace SocietyMng.Data.Entities
{
    public class Staff
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string BankAccount { get; set; }
    }
}