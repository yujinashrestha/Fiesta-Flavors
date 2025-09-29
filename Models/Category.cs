namespace Fiesta_Flavors.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
     
        public ICollection<Product> Products { get; set; }//A category has many products
    }
}