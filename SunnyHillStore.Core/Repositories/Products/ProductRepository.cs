using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Repositories.Products
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context, ICurrentUserHelper currentUserHelper) : base(context, currentUserHelper)
        {
        }
    }
}
