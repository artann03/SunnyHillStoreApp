using SunnyHillStore.Core.Repositories.Products;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;
using System.Threading.Tasks;

namespace SunnyHillStore.Core.Services.Products
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(
            IProductRepository productRepository,
            ICurrentUserHelper currentUserService) : base(productRepository, currentUserService)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
            return product;
        }
    }
}
