using KioskApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.ViewModels
{
    public class CreateCustomerViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<SelectListItem> Vendors { get; set; }
    }
}
