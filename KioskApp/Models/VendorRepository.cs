using KioskApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Models
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public VendorRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Vendor> Vendors => _applicationDbContext.Vendors;
    }
}
