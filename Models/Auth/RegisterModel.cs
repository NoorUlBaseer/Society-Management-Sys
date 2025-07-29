using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SocietyMng.Models.Auth
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords don't match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        public string SelectedRole { get; set; }

        public IEnumerable<SelectListItem> GetRoleOptions()
        {
            return new List<SelectListItem>
            {
                   new SelectListItem { Value = "Resident", Text = "Resident" },
                   new SelectListItem { Value = "Buyer", Text = "Buyer" }
            };
        }
    }
}