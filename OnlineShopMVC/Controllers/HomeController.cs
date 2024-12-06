using Microsoft.AspNetCore.Mvc;
using OnlineShopMVC.Models;
using System.Diagnostics;
using OnlineShopMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace OnlineShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch featured products or all products
            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .Take(6) // Show first 6 products
                .ToListAsync();

            // Fetch categories for dropdown
            var categories = await _context.Categories.ToListAsync();

            // Create a view model to pass both products and categories
            var viewModel = new HomeIndexViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult NotFound(int? code)
        {
            Response.StatusCode = code ?? 404;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
