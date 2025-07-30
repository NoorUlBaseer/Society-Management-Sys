using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Models.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(12, MinimumLength = 5, ErrorMessage = "Phone number must be between 5-12 characters.")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$",  //no alphabets
        ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The passwords don't match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = "Other";

        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Account Type")]
        public string SelectedRole { get; set; }

        [Display(Name = "Show Password")]
        public bool ShowPassword { get; set; }

        public IEnumerable<SelectListItem> GetRoleOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Resident", Text = "Resident" },
                new SelectListItem { Value = "Buyer", Text = "Buyer" }
            };
        }

        public IEnumerable<SelectListItem> GenderOptions => new List<SelectListItem>
        {
            new SelectListItem { Value = "Male", Text = "Male" },
            new SelectListItem { Value = "Female", Text = "Female" },
            new SelectListItem { Value = "Other", Text = "Other" }
        };
    }
}