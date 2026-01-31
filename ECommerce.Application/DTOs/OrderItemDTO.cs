namespace ECommerce.Application.DTOs
{
    public class OrderItemDTO
    {
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public string? ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
    }
}