using KioskApp.Controllers;
using KioskApp.Models;
using KioskApp.Test.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace KioskApp.Test
{
    public class ProductControllerTests
    {
        //[Fact]
        //public void IndexReturnsAViewContainsAllProducts()
        //{
        //    //Arrange
        //    var mockProductRepository = RepositoryMocks.GetProductRepository();
        //    var productController = new ProductController(mockProductRepository.Object);

        //    //Act
        //    var result = productController.List();

        //    //Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var products = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.ViewData.Model);
        //    Assert.Equal(1, products.Count());
        //}
    }
}
