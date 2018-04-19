using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductbyId(int productID);
        IEnumerable<Product> GetAllProductsBySeller(int vendorId);
    }
}
