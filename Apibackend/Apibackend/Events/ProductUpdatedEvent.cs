namespace Apibackend.Events
{
    public class ProductUpdatedEvent
    {
        public int ProductId { get; }
        public string Name { get; }
        public decimal Price { get; }

        public ProductUpdatedEvent(int productId, string name, decimal price)
        {
            ProductId = productId;
            Name = name;
            Price = price;
        }
    }
}
