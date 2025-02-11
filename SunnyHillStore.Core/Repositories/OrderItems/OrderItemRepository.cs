using Microsoft.EntityFrameworkCore;
using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Infrastructure.Repositories.OrderItems
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context, ICurrentUserHelper currentUserHelper) : base(context, currentUserHelper)
        {
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId && !oi.IsDeleted)
                .ToListAsync();
        }
    }
} 