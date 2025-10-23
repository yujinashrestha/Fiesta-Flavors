using Fiesta_Flavors.Data;
using Fiesta_Flavors.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace Fiesta_Flavors.Controllers
{
    public class ProductController : Controller
    {
        private Repository<Product> products;
        private Repository<Ingredient> ingredients;
        private Repository<Category> categories;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(MyAppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(context);
            ingredients = new Repository<Ingredient>(context);
            categories = new Repository<Category>(context);
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Ingredients = await ingredients.GetAllAsync();
            ViewBag.Categories = await categories.GetAllAsync();

            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {
                ViewBag.Operation = "Edit";
                var product = await products.GetIdAsync(id, new QueryOptions<Product> { Includes="ProductIngredients, Category"});

                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product, int[] ingredientIds, int catId)
        {
            ViewBag.Ingredients = await ingredients.GetAllAsync();
            ViewBag.Categories = await categories.GetAllAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Handle image upload
            if (product.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }
                product.ImageUrl = uniqueFileName;
            }

            // ---------- ADD NEW PRODUCT ----------
            if (product.ProductId == 0)
            {
                product.CategoryId = catId;

                // Save first to generate ProductId
                await products.AddAsync(product);

                // Now add ingredients with correct ProductId
                product.ProductIngredients = new List<ProductIngredient>();
                foreach (int id in ingredientIds)
                {
                    product.ProductIngredients.Add(new ProductIngredient
                    {
                        IngredientId = id,
                        ProductId = product.ProductId
                    });
                }

                // Update to save ingredient relationships
                await products.UpdateAsync(product);

                TempData["Success"] = $"Product '{product.Name}' added with ID: {product.ProductId}";
                return RedirectToAction("Index", "Product");
            }

            // ---------- UPDATE EXISTING PRODUCT ----------
            var existingProduct = await products.GetIdAsync(product.ProductId, new QueryOptions<Product> { Includes = "ProductIngredients" });

            if (existingProduct == null)
            {
                ModelState.AddModelError("", "Product not found.");
                return View(product);
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.CategoryId = catId;

            // Replace old ingredients
            existingProduct.ProductIngredients?.Clear();
            foreach (int id in ingredientIds)
            {
                existingProduct.ProductIngredients.Add(new ProductIngredient
                {
                    IngredientId = id,
                    ProductId = existingProduct.ProductId
                });
            }

            await products.UpdateAsync(existingProduct);

            TempData["Success"] = $"Product '{existingProduct.Name}' updated with ID: {existingProduct.ProductId}";
            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await products.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Product not found");
                return RedirectToAction("Index");
            }
        }

    }
}