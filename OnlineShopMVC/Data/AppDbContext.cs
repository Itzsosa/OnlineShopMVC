using Microsoft.EntityFrameworkCore;
using OnlineShopMVC.Models;

namespace OnlineShopMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OnlineShopMVC.Models.Category> Categories { get; set; } = default!;
        public DbSet<OnlineShopMVC.Models.Product> Products { get; set; } = default!;
        public DbSet<OnlineShopMVC.Models.Cart> Cart { get; set; } = default!;
        public DbSet<OnlineShopMVC.Models.CartItem> CartItems { get; set; } = default!;


    }
}
