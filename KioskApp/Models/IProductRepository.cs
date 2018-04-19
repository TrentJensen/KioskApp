using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetAllProductsByVendor(int vendorId);
        Product GetProductbyId(int productID);
    }
}