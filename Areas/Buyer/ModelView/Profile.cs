using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Areas.Buyer.ModelView
{
    public class Profile
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]+@[a-zA-Z]+\.com$",
            ErrorMessage = "Email must be in format: example@domain.com")]
        public string Email { get; set; }

        [RegularExpression(@"^(?!0+$)\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$",
            ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        [Compare("ConfirmNewPassword", ErrorMessage = "Passwords do not match")]
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
        public string RoleId { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}