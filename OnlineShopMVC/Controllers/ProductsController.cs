using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopMVC.Data;
using OnlineShopMVC.Models;
using System.IO;


namespace OnlineShopMVC.Controllers
{
    
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Include(p => p.Category)
                .ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Manejar carga de imagen
                if (product.ImageFile != null)
                {
                    // Validar tipo y tamaño de imagen
                    if (!ValidateImageFile(product.ImageFile))
                    {
                        ModelState.AddModelError("ImageFile", "Invalid file. Max size is 5MB. Allowed types: jpg, jpeg, png, webp.");
                        ViewBag.Categories = _context.Categories.ToList();
                        return View(product);
                    }

                    // Generar nombre único
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

                    // Crear carpeta si no existe
                    Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Guardar archivo
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    // Guardar ruta en modelo
                    product.ImagePath = "/images/products/" + uniqueFileName;
                    product.ImageFileName = uniqueFileName;
                }
                else
                {
                    // Imagen por defecto si no se sube imagen
                    product.ImagePath = "/images/default-product.png";
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el producto existente
                    var existingProduct = await _context.Products.FindAsync(id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Actualizar propiedades base
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Stock = product.Stock;

                    // Manejar carga de nueva imagen
                    if (product.ImageFile != null)
                    {
                        // Validar tipo y tamaño de imagen
                        if (!ValidateImageFile(product.ImageFile))
                        {
                            ModelState.AddModelError("ImageFile", "Invalid file. Max size is 5MB. Allowed types: jpg, jpeg, png, webp.");
                            ViewBag.Categories = _context.Categories.ToList();
                            return View(product);
                        }

                        // Eliminar imagen anterior si existe
                        if (!string.IsNullOrEmpty(existingProduct.ImagePath) &&
                            existingProduct.ImagePath != "/images/default-product.png")
                        {
                            DeleteImageFile(existingProduct.ImagePath);
                        }

                        // Generar nuevo nombre de archivo
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Guardar nuevo archivo
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }

                        // Actualizar ruta de imagen
                        existingProduct.ImagePath = "/images/products/" + uniqueFileName;
                        existingProduct.ImageFileName = uniqueFileName;
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Eliminar imagen si no es la imagen por defecto
            if (!string.IsNullOrEmpty(product.ImagePath) &&
                product.ImagePath != "/images/default-product.png")
            {
                DeleteImageFile(product.ImagePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        [Authorize(Roles = "Admin")]
        // Método para validar archivos de imagen
        private bool ValidateImageFile(IFormFile file)
        {
            if (file == null) return false;

            // Validar tamaño (5MB)
            if (file.Length > 5 * 1024 * 1024) return false;

            // Validar extensiones
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        [Authorize(Roles = "Admin")]
        // Método para eliminar archivo de imagen
        private void DeleteImageFile(string imagePath)
        {
            // Convertir ruta relativa a ruta física
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath.TrimEnd('\\'), imagePath.TrimStart('/').Replace('/', '\\'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        ///
        /// CATEGORY FILTERING
        /// 
        public async Task<IActionResult> Category(int categoryId)
        {
            var productsInCategory = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();

            ViewBag.CategoryName = _context.Categories.Find(categoryId)?.Name;
            return View(productsInCategory);
        }


        //Product Details from Client View

        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Home");
            }

            var searchResults = _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToList();

            var viewModel = new SearchResultViewModel
            {
                Query = query,
                Products = searchResults
            };

            return View(viewModel);
        }
    }
}
