using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;

public interface IOrderService : IBaseService<Order>
{
    Task<Order> CreateOrderAsync(int userId, IEnumerable<OrderItem> orderItems);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
    Task<Order> GetOrderByNumberAsync(string orderNumber);
} 