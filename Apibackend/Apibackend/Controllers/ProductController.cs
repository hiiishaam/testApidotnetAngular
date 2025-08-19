using Apibackend.DTOS;
using Apibackend.Models;
using Apibackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apibackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var products = _productService.GetAllProduct();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound($"Produit avec ID {id} introuvable.");
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        // POST: api/products
        [HttpPost]
        public ActionResult Create(Product product)
        {
            try
            {
                var createdProduct = _productService.CreateProduct(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne lors de la création : {ex.Message}");
            }
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public ActionResult Update(int id, Product product)
        {
            try
            {
                var updatedProduct = _productService.UpdateProduct(id, product);
                if (updatedProduct == null)
                    return NotFound($"Produit avec ID {id} introuvable.");
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne lors de la mise à jour : {ex.Message}");
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _productService.DeleteProduct(id);

            if (!result)
                return NotFound(new { message = $"Produit avec ID {id} introuvable." });

            return Ok(new ResponseMessage { Message = "Produit supprimé avec succès" });

        }





    }
}
