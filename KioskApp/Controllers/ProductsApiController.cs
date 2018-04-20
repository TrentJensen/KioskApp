using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KioskApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace KioskApp.Controllers
{
    [Route("api/[Controller]")]
    public class ProductsApiController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsApiController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_productRepository.GetAllProducts());
            }
            catch
            {
                return BadRequest("Failed to get products");
            }
        }

        
    }
}