using KioskApp.Data;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<Product> GetAllProductsByVendor(string vendorGuid)
        {
            return _appDbContext.Products.Where(p => p.VendorGuid == vendorGuid);
        }

        public IEnumerable<Product> GetAllProductsByVendorId(int id)
        {
            return _appDbContext.Products.Where(p => p.VendorId == id);
        }

        public void UpdateProduct(Product product)
        {
            _appDbContext.Products.Add(product);
            _appDbContext.Entry(product).State = EntityState.Modified;
            _appDbContext.SaveChanges();
        }
    }
}
