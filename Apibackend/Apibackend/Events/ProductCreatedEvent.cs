namespace Apibackend.Events
{
    public class ProductCreatedEvent
    {
        public int ProductId { get; }
        public string Name { get; }

        public ProductCreatedEvent(int productId, string name)
        {
            ProductId = productId;
            Name = name;
        }
    }
}
