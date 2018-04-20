using KioskApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public OrderRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void AddOrder(Order order)
        {
            _applicationDbContext.Orders.Add(order);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _applicationDbContext.Orders
                .Include(o => o.OrderLines).ToList();
        }

        public Order GetOrderById(int id)
        {
            return _applicationDbContext.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Order> GetOrdersByCustomerId(int customerId)
        {
            return _applicationDbContext.Orders.Where(p => p.CustomerId == customerId);
        }

        public IEnumerable<Order> GetOrdersByVendorId(int vendorId)
        {
            return _applicationDbContext.Orders.Where(p => p.SellerId == vendorId);
        }

        public bool SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}
