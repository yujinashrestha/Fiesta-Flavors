using Fiesta_Flavors.Data;
using Fiesta_Flavors.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

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
    }
}
