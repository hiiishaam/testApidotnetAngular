using Apibackend.Data;
using Apibackend.Events;
using Apibackend.Models;
using System;

namespace Apibackend.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProduct()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Find(id);
        }

        public Product CreateProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            // Émettre un event
            DispatchEvent(new ProductCreatedEvent(product.Id, product.Name));

            return product;
        }

        public Product UpdateProduct(int id, Product updatedProduct)
        {
            var product = _context.Products.Find(id);
            if (product == null) return null;

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            _context.SaveChanges();

            // Émettre un event
            DispatchEvent(new ProductUpdatedEvent(product.Id, product.Name, product.Price));

            return product;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            _context.SaveChanges();

            // Émettre un event
            DispatchEvent(new ProductDeletedEvent(id));

            return true;
        }

        // Méthode simplifiée pour dispatcher les events
        private void DispatchEvent(object domainEvent)
        {
            // Ici tu peux logger, envoyer à un bus, ou notifier d'autres services
            Console.WriteLine($"Domain Event déclenché : {domainEvent.GetType().Name}");
        }
    }
}
