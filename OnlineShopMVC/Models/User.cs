using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OnlineShopMVC.Models
{
    public class User
    {
        [Key] public int UserId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8)]
        [DisplayName("Password")]
        public string PasswordHash { get; set; }

        [DisplayName("FirstName")] public string FirstName { get; set; }

        [DisplayName("Lastname")] public string LastName { get; set; }

        [DisplayName("PhoneNumber")] public string PhoneNumber { get; set; }

        [DisplayName("UserRole")] public string? UserRole { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
