using AutoMapper;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Services.Orders
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICurrentUserHelper currentUserService,
            IMapper mapper) : base(orderRepository, currentUserService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, IEnumerable<OrderItem> orderItems)
        {
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                TotalAmount = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                Status = "Completed"
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            foreach (var item in orderItems)
            {
                item.OrderId = createdOrder.Id;
                await _orderItemRepository.CreateAsync(item);
            }

            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByNumberAsync(string orderNumber)
        {
            var order = await _orderRepository.GetOrderByNumberAsync(orderNumber);
            return _mapper.Map<OrderDto>(order);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
} 