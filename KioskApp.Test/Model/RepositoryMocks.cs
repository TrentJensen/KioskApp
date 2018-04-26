using System;
using System.Collections.Generic;
using System.Text;
using KioskApp.Models;
using Moq;

namespace KioskApp.Test.Model
{
    class RepositoryMocks
    {
        public static Mock<IProductRepository> GetProductRepository()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Test Cloth",
                    Price = 12.99M,
                    Category = new Category {CategoryName = "Microfiber" },
                    Description = "An anti-microbial cloth",
                    UnitsInStock = 4,
                    Color = ColorEnum.Lavender,
                    VendorId = 1,
                    VendorGuid = "4310be48-b16e-4054-af67-50d60bfe78f6",
                    IsLimitedEdition = true
                }
            };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(repo => repo.GetAllProducts()).Returns(products);
            mockProductRepository.Setup(repo => repo.GetProductbyId(It.IsAny<int>())).Returns(products[0]);
            return mockProductRepository;
        }
    }
}
