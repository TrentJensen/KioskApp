using KioskApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<OrderList> OrderLists { get; set; }
    }
}
