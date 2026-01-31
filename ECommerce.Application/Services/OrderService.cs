using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Services;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderDomainService _orderDomainService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public OrderService(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
             ICustomerRepository customerRepository,
            OrderDomainService orderDomainService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _orderDomainService = orderDomainService;
            _mapper = mapper;
        }

        public async Task<int> PlaceOrderAsync(CreateOrderRequestDTO request)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new Exception($"Customer with Id {request.CustomerId} does not exist.");
            var shippingAddress = new Address(request.ShippingAddress.Street,
                            request.ShippingAddress.City,
                            request.ShippingAddress.State,
                            request.ShippingAddress.PostalCode,
                            request.ShippingAddress.Country);
            var order = new Order(request.CustomerId, shippingAddress);

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with Id {item.ProductId} not found.");
                order.AddItem(product, item.Quantity);
            }

            // Use the domain service to validate order
            if (!_orderDomainService.CanPlaceOrder(customer, order.Items.ToList()))
                throw new Exception("Order cannot be placed due to domain validation failure.");
            await _orderRepository.AddAsync(order);

            return order.Id;  // Return generated Id
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            return _mapper.Map<OrderDTO>(order);
        }
    }
}