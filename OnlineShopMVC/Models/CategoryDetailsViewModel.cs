using System.Collections.Generic;
using OnlineShopMVC.Models;

namespace OnlineShopMVC.ViewModels
{
    public class CategoryDetailsViewModel
    {
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    }
}