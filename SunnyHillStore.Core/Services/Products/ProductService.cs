using SunnyHillStore.Core.Repositories.Products;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Services.Products
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(IProductRepository repository) : base(repository)
        {
        }
    }
}
