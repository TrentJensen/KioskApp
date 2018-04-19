using KioskApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _appDbContext;

        public ProductRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _appDbContext.Products;
        }

        public Product GetProductbyId(int productID)
        {
            return _appDbContext.Products.FirstOrDefault(p => p.Id == productID);
        }

        public IEnumerable<Product> GetAllProductsByVendor(int vendorId)
        {
            return _appDbContext.Products.Where(p => p.VendorId == vendorId);
        }
    }
}
