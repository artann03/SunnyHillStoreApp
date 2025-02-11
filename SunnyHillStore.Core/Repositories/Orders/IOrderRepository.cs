using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Model.Entities;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
    Task<Order> GetOrderByNumberAsync(string orderNumber);
} 