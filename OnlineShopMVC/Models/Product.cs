using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace OnlineShopMVC.Models
{
    public class Product
    {
        [Key]
        [Column("ProductId")]
        [DisplayName("Product ID")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [DisplayName("Product Name")]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        [DisplayName("Product Description")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        [DisplayName("Product Price")]
        public decimal Price { get; set; }

        [Required]
        [DisplayName("Category ID")]
        public int CategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        [DisplayName("Stock Quantity")]
        public int Stock { get; set; } = 0;

        [DisplayName("Image Path")]
        [StringLength(255)]
        public string? ImagePath { get; set; }

        [DisplayName("Image File Name")]
        [StringLength(100)]
        public string? ImageFileName { get; set; }

        // Propiedad para manejo de carga de archivos (no mapea a base de datos)
        [NotMapped]
        [DisplayName("Product Image")]
        public IFormFile? ImageFile { get; set; }

        [DisplayName("Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property for Category
        [DisplayName("Product Category")] public Category? Category { get; set; }
    }
}