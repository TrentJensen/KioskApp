using KioskApp.Models;
using System;
using Xunit;

namespace KioskApp.Test.Model
{
    public class ProductTests
    {
        [Fact]
        public void CanCreateProduct()
        {
            //Arrange
            var product = new Product();
            //Act

            //Assert
            Assert.NotNull(product);
        }

        [Fact]
        public void CanUpdateProductName()
        {
            //Arrange
            var product = new Product { Name = "Test Product", Price = 12.99M };
            //Act
            product.Name = "Another product";
            //Assert
            Assert.Equal("Another product", product.Name);
        }
    }
}
