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
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderRepository orderRepository, ICustomerRepository customerRepository,
            IProductRepository productRepository, IVendorRepository vendorRepository, UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _vendorRepository = vendorRepository;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            //Get the Guid of the current user from the UserManager
            var userGuid = _userManager.GetUserId(HttpContext.User);
            var customer = _customerRepository.GetCustomerByGuid(userGuid);
            var vendor = _vendorRepository.Vendors.FirstOrDefault(v => v.LoginId == userGuid);

            OrderViewModel orderViewModel = new OrderViewModel();

            if (customer != null)
            {
                IEnumerable<Order> orders = _orderRepository.GetOrdersByCustomerId(customer.Id);
                foreach (Order order in orders)
                {
                    IEnumerable<OrderList> orderLists = _orderRepository.GetOrderListsByOrderId(order.Id);
                    foreach (OrderList list in orderLists)
                    {
                        Product product = _productRepository.GetProductbyId(list.ProductId);
                        list.Product.Name = product.Name;
                    }
                    order.OrderLines = orderLists.ToList();
                }
                orderViewModel.Orders = orders;
            }

            if (vendor != null)
            {
                IEnumerable<Order> orders = _orderRepository.GetOrdersByVendorId(vendor.Id);
                foreach (Order order in orders)
                {
                    IEnumerable<OrderList> orderLists = _orderRepository.GetOrderListsByOrderId(order.Id);
                    foreach (OrderList list in orderLists)
                    {
                        Product product = _productRepository.GetProductbyId(list.ProductId);
                        list.Product.Name = product.Name;
                    }
                    order.OrderLines = orderLists.ToList();
                }
                orderViewModel.Orders = orders;
            }
                return View(orderViewModel);
        }
    }
}