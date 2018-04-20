using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KioskApp.Data;
using KioskApp.Models;
using KioskApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KioskApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShoppingCart _shoppingCart;
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductsController(IProductRepository productRespository, UserManager<ApplicationUser> userManager,
            ShoppingCart shoppingCart, ApplicationDbContext applicationDbContext)
        {
            _productRepository = productRespository;
            _userManager = userManager;
            _shoppingCart = shoppingCart;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(HttpContext.User);
            //var test = this.User.Identity.Name; //Get's current user's email

            //Get the Guid of the current vendor from the UserManager and use that to get only the vendor's products
            var userGuid = _userManager.GetUserId(HttpContext.User);
            var sellerProducts = _productRepository.GetAllProductsByVendor(userGuid);
            var userId = user.Id;

            var productsViewModel = new ProductsViewModel()
            {
                VendorName = $"{user.Id}'s Page",
                Products = sellerProducts.ToList()
            };

            return View(productsViewModel);
        }

        [HttpPost]
        public IActionResult AddProduct(AddToCartViewModel model)
        {
            if (model.Product.UnitsInStock >= model.Quantity)
            {
                _shoppingCart.AddToCart(model.Product, model.Quantity);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductbyId(id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}