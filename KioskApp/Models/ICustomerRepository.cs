﻿using System.Collections.Generic;

namespace KioskApp.Models
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerByGuid(string guid);
        IEnumerable<Customer> GetCustomersByVendor(int vendorId);
    }
}