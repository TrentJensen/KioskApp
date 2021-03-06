﻿using KioskApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.ViewModels
{
    public class ProductsViewModel
    {
        public string VendorName { get; set; }
        public List<ProductListItemViewModel> Products { get; set; }
        public string CurrentCategory { get; set; }
    }
}
