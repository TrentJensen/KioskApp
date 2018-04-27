using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KioskApp.Models;
using KioskApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KioskApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShoppingCart _shoppingCart;
        private Customer guestCustomer { get; set; }

        public ShoppingCartController(IProductRepository productRepository, IVendorRepository vendorRepository, IOrderRepository orderRepository, 
            ICustomerRepository customerRepository, UserManager<ApplicationUser> userManager, ShoppingCart shoppingCart)
        {
            _productRepository = productRepository;
            _vendorRepository = vendorRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _userManager = userManager;
            _shoppingCart = shoppingCart;
        }

        public IActionResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(shoppingCartViewModel);
        }

        [HttpPost]
        public IActionResult Checkout(ShoppingCartViewModel shoppingCartViewModel)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                return View(shoppingCartViewModel);
            }

            //Find the correct customer using the Identity Guid
            var customerGuid = _userManager.GetUserId(HttpContext.User);
            Customer cust = _customerRepository.GetCustomerByGuid(customerGuid);

            if (cust == null && guestCustomer == null)
            {
                return RedirectToAction("GuestCheckout");
            }

            //if the user is a registered customer            
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = cust.Id,
                VendorId = cust.VendorId,
                Total = shoppingCartViewModel.ShoppingCartTotal
            };

            _orderRepository.CreateOrder(order);

            foreach (var item in _shoppingCart.ShoppingCartItems)
            {
                //Subtract quantity from units in stock and save to db
                Product product = _productRepository.GetProductbyId(item.Product.Id);
                product.UnitsInStock -= item.Amount;
                _productRepository.UpdateProduct(product);
            }

            _shoppingCart.ClearCart();

            return RedirectToAction("CheckoutComplete");
        }

        public IActionResult GuestCheckout()
        {
            Customer cust = new Customer();
            return View(cust);
        }

        [HttpPost]
        public IActionResult GuestCheckout(Customer cust)
        {
            _customerRepository.AddCustomer(cust);
            Customer customer = _customerRepository.GetCustomerById(cust.Id);

            //Get the Guid of the current vendor from the UserManager
            var userGuid = _userManager.GetUserId(HttpContext.User);
            var vendor = _vendorRepository.Vendors.FirstOrDefault(v => v.LoginId == userGuid);

            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            Order order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.Now,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                VendorId = vendor.Id,
                Total = shoppingCartViewModel.ShoppingCartTotal
            };

            _orderRepository.CreateOrder(order);

            foreach (var item in _shoppingCart.ShoppingCartItems)
            {
                //Subtract quantity from units in stock and save to db
                Product product = _productRepository.GetProductbyId(item.Product.Id);
                product.UnitsInStock -= item.Amount;
                _productRepository.UpdateProduct(product);
            }
            _shoppingCart.ClearCart();

            return RedirectToAction("CheckoutComplete");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(ShoppingCartItem item)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            ShoppingCartItem selectedItem = items.FirstOrDefault(s => s.Id == item.Id);
            _shoppingCart.RemoveFromCart(selectedItem.Product);
            return RedirectToAction("Index");
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Thanks for your order.  You will hear from us soon!";
            return View();
        }
    }
}