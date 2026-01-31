using ECommerce.Domain.Entities;
namespace ECommerce.Domain.Services
{
    public class OrderDomainService
    {
        public bool CanPlaceOrder(Customer customer, List<OrderItem> items)
        {
            return customer != null && items != null && items.Count > 0;
        }
    }
}