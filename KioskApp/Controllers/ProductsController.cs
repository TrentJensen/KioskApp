using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KioskApp.Data;
using KioskApp.Models;
using KioskApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            //Get the Guid of the current vendor from the UserManager and use that to get only the vendor's products
            var userGuid = _userManager.GetUserId(HttpContext.User);

            ////If the user isn't logged in, redirect to an info page
            //if (userGuid == null)
            //{
            //    return RedirectToAction("ProductWarning");
            //}

            var sellerProducts = _productRepository.GetAllProductsByVendor(userGuid);
            //var userId = user.Id;
            var userId = "test";

            List<ProductListItemViewModel> prodList = new List<ProductListItemViewModel>();

            foreach(Product prod in sellerProducts)
            {
                prodList.Add(new ProductListItemViewModel
                {
                    Product = prod,
                    Quantity = prod.UnitsInStock
                });
            }

            var productsViewModel = new ProductsViewModel()
            {
                VendorName = $"{userId}'s Page",
                Products = prodList
            };

            return View(productsViewModel);
        }

        public IActionResult ProductWarning()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProductToCart(ProductToCartViewModel model)
        {
            Product product = _productRepository.GetProductbyId(model.ProductId);
            if (product.UnitsInStock >= model.Quantity)
            {
                _shoppingCart.AddToCart(product, model.Quantity);

                //Subtract quantity from units in stock and save to db
                product.UnitsInStock -= model.Quantity;
                _applicationDbContext.Products.Add(product);
                _applicationDbContext.Entry(product).State = EntityState.Modified;
                _applicationDbContext.SaveChanges();
            }

            //Subtract quantity from units in stock and save to db
            product.UnitsInStock -= model.Quantity;
            _applicationDbContext.Products.Add(product);
            _applicationDbContext.Entry(product).State = EntityState.Modified;
            _applicationDbContext.SaveChanges();
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