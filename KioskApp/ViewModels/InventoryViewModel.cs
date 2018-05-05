using KioskApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.ViewModels
{
    public class InventoryViewModel
    {
		public IEnumerable<Product> Products { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }

	}
}
