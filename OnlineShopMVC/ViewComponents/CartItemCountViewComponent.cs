using Microsoft.AspNetCore.Mvc;
using OnlineShopMVC.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace OnlineShopMVC.ViewComponents
{
    public class CartItemCountViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartItemCountViewComponent(
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Obtener el HttpContext actual
            var httpContext = _httpContextAccessor.HttpContext;

            // Verificar si el usuario está autenticado
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return View(0);
            }

            // Obtener el ID de usuario
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return View(0);
            }

            // Obtener el conteo de elementos del carrito
            int cartItemCount = await _cartService.GetCartItemCountAsync(userId);
            return View(cartItemCount);
        }
    }
}