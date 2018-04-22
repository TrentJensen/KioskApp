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
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShoppingCart _shoppingCart;
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductsController(IProductRepository productRespository, ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager,
            ShoppingCart shoppingCart, ApplicationDbContext applicationDbContext)
        {
            _productRepository = productRespository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _shoppingCart = shoppingCart;
            _applicationDbContext = applicationDbContext;
        }

        public ViewResult List(string category)
        {
            //Get the Guid of the current vendor from the UserManager and use that to get only the vendor's products

            //var userGuid = _userManager.GetUserId(HttpContext.User);
            var userGuid = "4310be48-b16e-4054-af67-50d60bfe78f6";

            ////If the user isn't logged in, redirect to an info page
            //if (userGuid == null)
            //{
            //    return RedirectToAction("ProductWarning");
            //}

            //var userId = user.Id;
            IEnumerable<Product> vendorProducts = _productRepository.GetAllProductsByVendor(userGuid)
                    .OrderBy(p => p.Name);

            //Get all of the products for the current category
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(category))
            {

                currentCategory = "All products";
            }
            else
            {
                int categoryId = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).Id;
                vendorProducts = vendorProducts.Where(p => p.CategoryId == categoryId)
                    .OrderBy(p => p.Name);
                currentCategory = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).CategoryName;
            }

            List<ProductListItemViewModel> prodList = new List<ProductListItemViewModel>();

            foreach(Product prod in vendorProducts)
            {
                prodList.Add(new ProductListItemViewModel
                {
                    Product = prod,
                    Quantity = prod.UnitsInStock
                });
            }

            var productsViewModel = new ProductsViewModel()
            {
                VendorName = $"Vendor's Page",
                Products = prodList,
                CurrentCategory = currentCategory
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

                ////Subtract quantity from units in stock and save to db
                //product.UnitsInStock -= model.Quantity;
                //_applicationDbContext.Products.Add(product);
                //_applicationDbContext.Entry(product).State = EntityState.Modified;
                //_applicationDbContext.SaveChanges();
            }

            return RedirectToAction("List");
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