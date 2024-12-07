using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopMVC.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public List<CartItem> CartItems { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }

    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }


    }
}