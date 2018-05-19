using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetAllProductsByVendor(string vendorGuid);
        IEnumerable<Product> GetAllProductsByVendorId(int id);
        Product GetProductbyId(int productID);
		void ChangeProductStockById(int productId, int quantity);
		void UpdateProduct(Product product);
    }
}