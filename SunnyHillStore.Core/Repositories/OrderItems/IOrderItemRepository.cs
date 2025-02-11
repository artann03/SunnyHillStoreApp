using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Model.Entities;

public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
} 