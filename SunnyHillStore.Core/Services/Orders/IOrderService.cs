using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;

public interface IOrderService : IBaseService<Order>
{
    Task<OrderDto> CreateOrderAsync(int userId, IEnumerable<OrderItem> orderItems);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
    Task<OrderDto> GetOrderByNumberAsync(string orderNumber);
} 