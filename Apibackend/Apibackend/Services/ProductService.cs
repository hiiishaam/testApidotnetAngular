using Apibackend.Data;
using Apibackend.Events;
using Apibackend.Hubs;
using Apibackend.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using static Apibackend.Hubs.Hubs;

namespace Apibackend.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductService(AppDbContext context, IHubContext<ProductHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

            // Émettre un event SignalR
            var evt = new ProductCreatedEvent(product.Id, product.Name);
            DispatchEvent("ProductCreated", evt);

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

            // Émettre un event SignalR
            var evt = new ProductUpdatedEvent(product.Id, product.Name, product.Price);
            DispatchEvent("ProductUpdated", evt);

            return product;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            _context.SaveChanges();

            // Émettre un event SignalR
            var evt = new ProductDeletedEvent(id);
            DispatchEvent("ProductDeleted", evt);

            return true;
        }

        // Envoi de l’event via SignalR
        private void DispatchEvent(string eventName, object domainEvent)
        {
            _hubContext.Clients.All.SendAsync(eventName, domainEvent);
            Console.WriteLine($"📢 Event envoyé : {eventName} -> {domainEvent}");
        }
    }
}
