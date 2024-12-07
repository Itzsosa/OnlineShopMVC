using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopMVC.Data;
using OnlineShopMVC.Models;
using OnlineShopMVC.Services;
using OnlineShopMVC.ViewModels;
namespace OnlineShopMVC.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;
        public CartsController(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }
        public async Task<IActionResult> Index()
        {
            // Extract user ID from claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Find or create cart for the user
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            var viewModel = new CartViewModel
            {
                CartItems = cart.CartItems ?? new List<CartItem>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string returnUrl = null)
        {
            try
            {
                // Extract user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    // Redirect to login with the current page as return URL
                    return RedirectToAction("Login", "Users", new { returnUrl = returnUrl ?? Url.Action("Index", "Home") });
                }

                int userId = int.Parse(userIdClaim.Value);

                // Validate product
                var product = await _context.Products
                    .SingleOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }

                // Validate quantity
                if (quantity <= 0)
                {
                    quantity = 1; // Default to 1 if invalid
                }

                // Find or create cart
                var cart = await _context.Cart
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CartItems = new List<CartItem>()
                    };
                    _context.Cart.Add(cart);
                }

                // Find existing cart item or create new
                var cartItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price,
                        CartId = cart.CartId
                    };
                    cart.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity += quantity;
                }

                await _context.SaveChangesAsync();

                // Success message
                TempData["SuccessMessage"] = $"{quantity} x {product.Name} added to cart!";

                // Redirect back to the original page or home
                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "An error occurred while adding the product to cart.";
                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            // Extract user ID from claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Find and remove cart item
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Optional: Add TempData message
            TempData["SuccessMessage"] = "Item removed from cart.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            // Extract user ID from claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Find cart item
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            await _context.SaveChangesAsync();

            // Optional: Add TempData message
            TempData["SuccessMessage"] = "Cart updated successfully.";

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> GetCartItemCount()
        {
            // Find the user ID claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Json(0);
            }

            // Parse the user ID
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(0);
            }

            // Get cart item count
            int cartItemCount = await _cartService.GetCartItemCountAsync(userId);
            return Json(cartItemCount);
        }
    }
}
