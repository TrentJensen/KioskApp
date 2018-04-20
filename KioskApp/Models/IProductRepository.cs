using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetAllProductsByVendor(string vendorGuid);
        Product GetProductbyId(int productID);
    }
}