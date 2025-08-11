using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]+@[a-zA-Z]+\.com$",
       ErrorMessage = "Email must be in format ammy@abc.com")]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Show password")]
        public bool ShowPassword { get; set; } = false;
    }
}