using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KioskApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace KioskApp.Controllers
{
    public class OrdersApiController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersApiController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_orderRepository.GetAllOrders());
            }
            catch
            {
                return BadRequest("Failed to get products");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _orderRepository.GetOrderById(id);
                if (order != null)
                    return Ok();
                else
                    return NotFound();
            }
            catch
            {
                return BadRequest("Failed to get products");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Order order)
        {
            try
            {
                _orderRepository.AddOrder(order);
                if (_orderRepository.SaveAll())
                {
                    return Created($"/api/ordersapi/{order.Id}", order);
                }

            }
            catch
            {

            }

            return BadRequest("Failed to save new order");
        }
    }
}