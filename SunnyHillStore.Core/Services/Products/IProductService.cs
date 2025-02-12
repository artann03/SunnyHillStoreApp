using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using SunnyHillStore.Model.Models.Products;

namespace SunnyHillStore.Core.Services.Products
{
    public interface IProductService : IBaseService<Product>
    {
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<Product> GetByPublicIdAsync(string publicId);
        Task<ProductResponseDto> CreateAsync(CreateProductDto createDto);
        Task<ProductResponseDto> UpdateAsync(string publicId, UpdateProductDto updateDto);
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<PaginatedResponseDto<ProductResponseDto>> GetFilteredAsync(ProductFilterRequestDto filter);
    }
}
