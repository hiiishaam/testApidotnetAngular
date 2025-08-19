using Apibackend.Controllers;
using Apibackend.DTOS;
using Apibackend.Models;
using Apibackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace testApi
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<IProductService> _mockService;
        private ProductsController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Mock de l'interface, plus de problème de constructeur
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [TestMethod]
        public void GetAll_ShouldReturnOkWithProducts()
        {
            _mockService.Setup(s => s.GetAllProduct()).Returns(new List<Product>
    {
        new Product { Id = 1, Name = "P1" }
    });

            var actionResult = _controller.GetAll(); // retourne IActionResult
            var okResult = actionResult as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var products = okResult.Value as List<Product>;
            Assert.IsNotNull(products);
            Assert.AreEqual(1, products.Count);
        }


        [TestMethod]
        public void GetById_ShouldReturnOk_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "P1" };
            _mockService.Setup(s => s.GetProductById(1)).Returns(product);

            var actionResult = _controller.GetById(1);
            var okResult = actionResult as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("P1", ((Product)okResult.Value).Name);
        }

        [TestMethod]
        public void GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _mockService.Setup(s => s.GetProductById(2)).Returns((Product)null);

            var actionResult = _controller.GetById(2);
            var notFoundResult = actionResult as NotFoundObjectResult;

            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void Create_ShouldReturnCreatedProduct()
        {
            var product = new Product { Id = 1, Name = "P2" };
            _mockService.Setup(s => s.CreateProduct(product)).Returns(product);

            var actionResult = _controller.Create(product);
            var createdResult = actionResult as CreatedAtActionResult;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(product, createdResult.Value);
        }

        [TestMethod]
        public void Update_ShouldReturnOk_WhenProductExists()
        {
            var updatedProduct = new Product { Id = 1, Name = "P2Modifié" };
            _mockService.Setup(s => s.UpdateProduct(1, updatedProduct)).Returns(updatedProduct);

            var actionResult = _controller.Update(1, updatedProduct);
            var okResult = actionResult as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("P2Modifié", ((Product)okResult.Value).Name);
        }

        [TestMethod]
        public void Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var updatedProduct = new Product { Id = 2, Name = "P3Modifié" };
            _mockService.Setup(s => s.UpdateProduct(2, updatedProduct)).Returns((Product)null);

            var actionResult = _controller.Update(2, updatedProduct);
            var notFoundResult = actionResult as NotFoundObjectResult;

            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void Delete_ShouldReturnOkMessage_WhenProductDeleted()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteProduct(It.IsAny<int>())).Returns(true);

            // Act
            IActionResult result = _controller.Delete(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ResponseMessage;
            Assert.IsNotNull(response);
            Assert.AreEqual("Produit supprimé avec succès", response.Message);
        }





        [TestMethod]
        public void Delete_ShouldReturnNotFound_WhenProductNotExist()
        {
            _mockService.Setup(s => s.DeleteProduct(2)).Returns(false);

            var result = _controller.Delete(2) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
