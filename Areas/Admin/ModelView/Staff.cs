using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Areas.Admin.ModelView
{
    public class StaffView
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Position { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        public bool IsActive { get; set; }

        [Display(Name = "Bank Account")]
        public string BankAccount { get; set; }

        [Phone]
        public string ContactNumber { get; set; }
    }

    public class SalaryUpdate
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Position { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal NewSalary { get; set; }
        public string Reason { get; set; } 
    }
}