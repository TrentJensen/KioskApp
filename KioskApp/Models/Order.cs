using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public ICollection<OrderList> OrderLines { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public double CouponDiscount { get; set; }

    }
}
