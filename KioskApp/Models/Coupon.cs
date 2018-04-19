using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public int SellerID { get; set; }
        public Vendor Vendor { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public double PercentDiscount { get; set; }
    }
}
