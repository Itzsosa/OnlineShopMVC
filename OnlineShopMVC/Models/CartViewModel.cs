using System.Collections.Generic;
using System.Linq;
using OnlineShopMVC.Models;

namespace OnlineShopMVC.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalPrice => CartItems.Sum(item => item.Price * item.Quantity);
        public int TotalItems => CartItems.Sum(item => item.Quantity);
    }
}
