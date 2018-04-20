using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal VendorCost { get; set; }
        public ColorEnum Color { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsLimitedEdition { get; set; }
        public string Image { get; set; }
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public string VendorGuid { get; set; }
    }
}
