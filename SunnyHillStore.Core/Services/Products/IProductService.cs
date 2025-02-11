using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;
using System.Threading.Tasks;

namespace SunnyHillStore.Core.Services.Products
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product> GetByIdAsync(int id);
        Task<Product> UpdateAsync(Product product);
    }
}
