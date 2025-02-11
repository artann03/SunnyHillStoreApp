using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Services.Orders
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICurrentUserHelper currentUserService) : base(orderRepository, currentUserService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Order> CreateOrderAsync(int userId, IEnumerable<OrderItem> orderItems)
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

            return createdOrder;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }

        public async Task<Order> GetOrderByNumberAsync(string orderNumber)
        {
            return await _orderRepository.GetOrderByNumberAsync(orderNumber);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
} 