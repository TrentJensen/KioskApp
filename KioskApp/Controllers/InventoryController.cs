using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KioskApp.Models;
using KioskApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KioskApp.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public InventoryController(IProductRepository productRepository, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Index(string searchString)
        {
			ViewData["CurrentFilter"] = searchString;

			var userGuid = _userManager.GetUserId(HttpContext.User);

			IEnumerable<Product> products = _productRepository.GetAllProductsByVendor(userGuid)
												.OrderBy(p => p.Name);
			if (!String.IsNullOrEmpty(searchString))
			{
				searchString = searchString.ToLower();
				products = products.Where(p => p.Name.ToLower().StartsWith(searchString));
			}
			InventoryViewModel inventoryVM = new InventoryViewModel
			{
				Products = products
			};
            return View(inventoryVM);
        }

		[HttpPost]
		public IActionResult AddProduct(InventoryViewModel model)
		{
			Product prod = _productRepository.GetProductbyId(model.ProductId);
			_productRepository.AddProductStockById(model.ProductId, model.Quantity);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult RemoveProduct(InventoryViewModel model)
		{
			Product prod = _productRepository.GetProductbyId(model.ProductId);
			_productRepository.RemoveProductStockById(model.ProductId, model.Quantity);
			return RedirectToAction("Index");
		}

    }
}
