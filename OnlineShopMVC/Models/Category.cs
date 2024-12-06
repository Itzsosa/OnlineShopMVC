using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OnlineShopMVC.Models
{
    public class Category
    {
        [Key]
        [DisplayName("Categories ID")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The category name must be between 2 and 50 characters.")]
        [DisplayName("Categories Name")]
        public string Name { get; set; }

        // Optional collection navigation property
        public ICollection<Product>? Products { get; set; }
    }
}