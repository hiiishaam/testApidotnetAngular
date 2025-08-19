using System.ComponentModel.DataAnnotations;

namespace Apibackend.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Product Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Product Description")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Product description is required.")]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Product Price")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [Required(ErrorMessage = "Product price is required.")]
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
