using Fiesta_Flavors.Data;
using Fiesta_Flavors.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta_Flavors.Controllers
{
    public class IngredientController : Controller
    {
        private Repository<Ingredient> ingredients;

        public IngredientController(MyAppDbContext context)
        {
            ingredients = new Repository<Ingredient>(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await ingredients.GetAllAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var ingredient = await ingredients.GetIdAsync(id, new QueryOptions<Ingredient>()
            {
                Includes = "ProductIngredients.Product"
            });

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // Ingredient/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientId, Name")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await ingredients.AddAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }

        // GET: Delete confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ingredient = await ingredients.GetIdAsync(id, new QueryOptions<Ingredient>
            {
                Includes = "ProductIngredients.Product"  // Fixed: was "ProductIngredients" (typo) and was using wrong type
            });

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Delete action
        [HttpPost, ActionName("Delete")]  // Fixed: Added ActionName attribute
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)  // Fixed: Changed method signature completely
        {
            await ingredients.DeleteAsync(id);  // Fixed: Call repository's DeleteAsync, not entity's
            return RedirectToAction("Index");
        }

        [HttpGet]

        public async Task<IActionResult> Edit(int id)
        {
            var ingredient = await ingredients.GetIdAsync(id, new QueryOptions<Ingredient> { Includes="ProductIngredients.Product"});
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("IngredientId, Name")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await ingredients.UpdateAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }
    }
}