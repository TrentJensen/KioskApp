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
        private readonly ShoppingCart _shoppingCart;

        public OrderRepository(ApplicationDbContext applicationDbContext, ShoppingCart shoppingCart)
        {
            _applicationDbContext = applicationDbContext;
            _shoppingCart = shoppingCart;
        }

        public void CreateOrder(Order order)
        {
            order.OrderDate = DateTime.Now;
            order.Total = _shoppingCart.GetShoppingCartTotal();

            _applicationDbContext.Orders.Add(order);

            var shoppingCartItems = _shoppingCart.ShoppingCartItems;

            foreach (var item in shoppingCartItems)
            {
                var orderList = new OrderList()
                {
                    Quantity = item.Amount,
                    ProductId = item.Product.Id,
                    OrderId = order.Id,
                    Price = item.Product.Price
                };

                _applicationDbContext.OrderLists.Add(orderList);
            }

            _applicationDbContext.SaveChanges();
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
            return _applicationDbContext.Orders.Where(o => o.CustomerId == customerId);
        }

        public IEnumerable<OrderList> GetOrderListsByOrderId(int orderId)
        {
            return _applicationDbContext.OrderLists.Where(o => o.OrderId == orderId);
        }

        public IEnumerable<Order> GetOrdersByVendorId(int vendorId)
        {
            return _applicationDbContext.Orders.Where(p => p.VendorId == vendorId);
        }

        public bool SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}
