using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetOrdersByCustomerId(int customerId);
        IEnumerable<Order> GetOrdersByVendorId(int vendorId);
        IEnumerable<OrderList> GetOrderListsByOrderId(int orderId);
        Order GetOrderById(int id);
        void CreateOrder(Order order);
        bool SaveAll();
    }
}