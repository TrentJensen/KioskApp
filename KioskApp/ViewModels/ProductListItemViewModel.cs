using KioskApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.ViewModels
{
    public class ProductListItemViewModel : IProductListItemViewModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
