namespace Fiesta_Flavors.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string? Name { get; set; }
        public ICollection<ProductIngredient>? ProductIngredients { get; set; }// An ingredient may be in many products
    }
}