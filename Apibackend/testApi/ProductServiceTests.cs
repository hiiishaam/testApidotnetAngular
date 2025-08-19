using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Apibackend.Data;
using Apibackend.Models;
using Apibackend.Services;
using System.Linq;

namespace testApi
{
    [TestClass]
    public class ProductServiceTests
    {
        private AppDbContext _context;
        private ProductService _service;

        [TestInitialize]
        public void Setup()
        {
            // Chaque test utilise une base InMemory unique grâce à Guid
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new ProductService(_context);
        }

        [TestMethod]
        public void CreateProduct_ShouldAddProduct()
        {
            var product = new Product { Name = "Produit1", Description = "Desc1", Price = 10 };

            var result = _service.CreateProduct(product);

            Assert.IsNotNull(result);
            Assert.AreEqual("Produit1", result.Name);
            Assert.AreEqual(1, _context.Products.Count());
        }

        [TestMethod]
        public void GetProductById_ShouldReturnProduct()
        {
            var product = new Product { Name = "Produit2", Description = "Desc2", Price = 20 };
            _service.CreateProduct(product);

            var result = _service.GetProductById(product.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Produit2", result.Name);
        }

        [TestMethod]
        public void UpdateProduct_ShouldModifyProduct()
        {
            var product = new Product { Name = "Produit3", Description = "Desc3", Price = 30 };
            _service.CreateProduct(product);

            var updated = new Product { Name = "Produit3Modifié", Description = "Desc3Modifié", Price = 35 };
            var result = _service.UpdateProduct(product.Id, updated);

            Assert.IsNotNull(result);
            Assert.AreEqual("Produit3Modifié", result.Name);
            Assert.AreEqual(35, result.Price);
        }

        [TestMethod]
        public void DeleteProduct_ShouldRemoveProduct()
        {
            var product = new Product { Name = "Produit4", Description = "Desc4", Price = 40 };
            _service.CreateProduct(product);

            var result = _service.DeleteProduct(product.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Products.Count());
        }
    }
}
