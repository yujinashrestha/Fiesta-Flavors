using Fiesta_Flavors.Data;
using Fiesta_Flavors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Fiesta_Flavors.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyAppDbContext _context;
        private Repository<Product> _products;
        private Repository<Order> _orders;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(MyAppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _products = new Repository<Product>(context);
            _orders = new Repository<Order>(context);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var modal = HttpContext.Session.Get<OrderViewModel>("OrderViewModel") ?? new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>(),
                Products = await _products.GetAllAsync()
            };
            return View(modal);

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddItem(int prodId, int prodqty)
        {
            var product = await _context.Products.FindAsync(prodId);
            if(product== null)
            {
                return NotFound();
            }

            //Retrieve or create an OrderViewModel from session or other state management

            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel") ?? new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>(),
                Products = await _products.GetAllAsync()
            };

            //check if the product is already in the order
            var existingItem = model.OrderItems.FirstOrDefault(oi => oi.ProductId == prodId);

            //If the product is alreawdy in the order, update the quantity

            if (existingItem != null)
            {
                existingItem.Quantity += prodqty;
            }
            else
            {
                model.OrderItems.Add(new OrderItemViewModel
                {
                    ProductId = product.ProductId,
                    Price=product.Price,
                    Quantity=prodqty,
                    ProductName=product.Name

                });
            }

            //update the total amount
            model.TotalAmount = model.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            //Save updated OrderViewModel to session
            HttpContext.Session.Set("OrderViewModel", model);
            //Redirect back to Create to show updated order items
            return RedirectToAction("Create", model);

        }
        public async Task<IActionResult> Cart()
        {
            //Retrive the OrderViewModel from the Session or other state management
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel");
            if(model==null || model.OrderItems.Count == 0)
            {
                return RedirectToAction("Create");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder()
        {
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel");
            if(model==null || model.OrderItems.Count == 0)
            {
                return RedirectToAction("Create");
            }

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                UserId = _userManager.GetUserId(User),
                OrderItems = new List<OrderItem>()
            };

            //Add OrderItems to the Order entity
            foreach(var item in model.OrderItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }
            //Save the Order entity to the database
            await _orders.AddAsync(order);
            //Clear the OrderViewModel from session or other state management
            HttpContext.Session.Remove("OrderViewModel");

            //Redirect to the Order Confirmation page
            return RedirectToAction("ViewOrder");
            
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewOrder()
        {
            var userId = _userManager.GetUserId(User);
            var userOrders = await
            _orders.GetAllByIdAsync(userId, "UserId", new QueryOptions<Order>
            {
                Includes = "OrderItems.Product"
            });
            return View(userOrders);
        }
    }
}
