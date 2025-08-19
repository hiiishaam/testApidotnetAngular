using Apibackend.Models;

namespace Apibackend.Services
{
    public interface IProductService
    {
        List<Product> GetAllProduct();
        Product GetProductById(int id);
        Product CreateProduct(Product product);
        Product UpdateProduct(int id, Product updatedProduct);
        bool DeleteProduct(int id);
    }
}
