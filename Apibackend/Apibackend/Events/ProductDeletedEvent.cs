namespace Apibackend.Events
{
    public class ProductDeletedEvent
    {
        public int ProductId { get; }

        public ProductDeletedEvent(int productId)
        {
            ProductId = productId;
        }
    }
}
