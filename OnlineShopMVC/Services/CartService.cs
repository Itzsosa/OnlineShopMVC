using Microsoft.EntityFrameworkCore;
using OnlineShopMVC.Data;

namespace OnlineShopMVC.Services
{

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            return await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }
    }
}
