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
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShoppingCart _shoppingCart;
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductController(IProductRepository productRespository, ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager,
            ShoppingCart shoppingCart, ApplicationDbContext applicationDbContext, IVendorRepository vendorRepository, ICustomerRepository customerRepository)
        {
            _productRepository = productRespository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _shoppingCart = shoppingCart;
            _applicationDbContext = applicationDbContext;
            _vendorRepository = vendorRepository;
            _customerRepository = customerRepository;
        }

        public ViewResult List(string category)
        {
            //Get the Guid of the current user from the UserManager
            var userGuid = _userManager.GetUserId(HttpContext.User);
            var customer = _customerRepository.GetCustomerByGuid(userGuid);
            IEnumerable<Product> vendorProducts;
            //If the current user is a vendor, show that vendor's products
            if (customer == null)
            {
                vendorProducts = _productRepository.GetAllProductsByVendor(userGuid)
                    .OrderBy(p => p.Name);
            }
            //If the current user is a customer, show the products of the customer's vendor
            else
            {
                vendorProducts = _productRepository.GetAllProductsByVendorId(customer.VendorId)
                    .OrderBy(p => p.Name);
            }

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