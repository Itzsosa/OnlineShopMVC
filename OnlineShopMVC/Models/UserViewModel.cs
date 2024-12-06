using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OnlineShopMVC.Models
{
    public class UserEditViewModel
    {
        [Display(Name = "First Name")]
        [StringLength(50)] // Optional: add any validation you want
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)] // Optional: add any validation you want
        public string? LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone] // Adds phone number validation
        public string? PhoneNumber { get; set; }
    }
}