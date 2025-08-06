using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Models.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
         ErrorMessage = "Full name should contain only letters and spaces")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]+@[a-zA-Z]+\.com$",
        ErrorMessage = "Email must be in format ammy@abc.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(11, MinimumLength = 6)]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@#]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters with 1 uppercase, 1 lowercase, 1 number, and 1 special character (*, @, or #)")]
        [StringLength(15, MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Gender { get; set; } = "Other";

        [Required]
        public string SelectedRole { get; set; }

        public bool ShowPassword { get; set; }
        public IEnumerable<SelectListItem> GetRoleOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Buyer", Text = "Buyer" },
                new SelectListItem { Value = "Sales", Text = "Seller" }               
            };
        }
        public IEnumerable<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Male", Text = "Male" },
            new SelectListItem { Value = "Female", Text = "Female" },
            new SelectListItem { Value = "Other", Text = "Other" }
        };
    }
}