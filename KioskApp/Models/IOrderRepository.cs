using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetOrdersByCustomerId(int customerId);
        IEnumerable<Order> GetOrdersByVendorId(int vendorId);
        Order GetOrderById(int id);
        void AddOrder(Order order);
        bool SaveAll();
    }
}