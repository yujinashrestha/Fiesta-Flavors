using Fiesta_Flavors.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fiesta_Flavors.Data
{
    public class MyAppDbContext:IdentityDbContext<ApplicationUser>
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            

            // Prevent EF from modifying Identity tables
           builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(ut => ut.LoginProvider).HasMaxLength(450);
                b.Property(ut => ut.Name).HasMaxLength(450);
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property(ul => ul.LoginProvider).HasMaxLength(450);
                b.Property(ul => ul.ProviderKey).HasMaxLength(450);
            });

            // Your decimal configurations here too:
            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        
        builder.Entity<ProductIngredient>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });

            builder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(pi => pi.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);

            builder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(pi => pi.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);

            //seed Data
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Appetizer" },
                new Category { CategoryId = 2, Name = "Entree" },
                new Category { CategoryId = 3, Name = "Side Dish" },
                new Category { CategoryId = 4, Name = "Dessert" },
                new Category { CategoryId = 5, Name = "Beverage" }
                );

            builder.Entity<Ingredient>().HasData(
                //Adding restaurant ingredient here
                new Ingredient { IngredientId =1, Name= "Buff"},
                new Ingredient { IngredientId = 2, Name = "Chicken" },
                new Ingredient { IngredientId = 3, Name = "Fish" },
                new Ingredient { IngredientId = 4, Name = "Tortilla" },
                new Ingredient { IngredientId = 5, Name = "Lettuce" },
                new Ingredient { IngredientId = 6, Name = "Tomato" }

                );

            builder.Entity<Product>().HasData(
                //Add restaurant food entries here
                new Product
                {
                    ProductId=1,
                    Name="Buff Taco",
                Description= "A delicious buff taco",
                Price=2.50m,
                Stock=100,
                CategoryId=2
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Chicken Taco",
                    Description = "A delicious chicken taco",
                    Price = 3.50m,
                    Stock = 101,
                    CategoryId = 2
                },

                new Product
                {
                    ProductId = 3,
                    Name = "Fish Taco",
                    Description = "A delicious fish taco",
                    Price = 3.99m,
                    Stock = 90,
                    CategoryId = 2
                }

                );

            builder.Entity<ProductIngredient>().HasData(
                new ProductIngredient { ProductId = 1, IngredientId = 1 },
                new ProductIngredient { ProductId = 1, IngredientId = 4 },
                new ProductIngredient { ProductId = 1, IngredientId = 5 },
                new ProductIngredient { ProductId = 1, IngredientId = 6 },
                new ProductIngredient { ProductId = 2, IngredientId = 2 },
                new ProductIngredient { ProductId = 2, IngredientId = 4 },
                new ProductIngredient { ProductId = 2, IngredientId = 5 },
                new ProductIngredient { ProductId = 2, IngredientId = 6 },
                new ProductIngredient { ProductId = 3, IngredientId = 3 },
                new ProductIngredient { ProductId = 3, IngredientId = 4 },
                new ProductIngredient { ProductId = 3, IngredientId = 5 },
                new ProductIngredient { ProductId = 3, IngredientId = 6 }
                );

        }


    }
}
