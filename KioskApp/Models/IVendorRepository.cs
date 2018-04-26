using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface IVendorRepository
    {
        IEnumerable<Vendor> Vendors { get; }
    }
}