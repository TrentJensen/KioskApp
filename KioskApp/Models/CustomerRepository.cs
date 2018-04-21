using KioskApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CustomerRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _applicationDbContext.Customers;
        }

        public Customer GetCustomerByGuid(string guid)
        {
            return _applicationDbContext.Customers.FirstOrDefault(p => p.LoginId == guid);
        }

        public IEnumerable<Customer> GetCustomersByVendor(int vendorId)
        {
            return _applicationDbContext.Customers.Where(p => p.VendorId == vendorId);
        }
    }
}
