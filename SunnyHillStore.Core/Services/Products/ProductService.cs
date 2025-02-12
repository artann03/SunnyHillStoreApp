using AutoMapper;
using SunnyHillStore.Core.Repositories.Products;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;
using SunnyHillStore.Model.Models.Products;

namespace SunnyHillStore.Core.Services.Products
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            ICurrentUserHelper currentUserService,
            IMapper mapper) : base(productRepository, currentUserService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> GetByPublicIdAsync(string publicId)
        {
            var product = await _productRepository.GetByPublicIdAsync(publicId);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto createDto)
        {
            var product = _mapper.Map<Product>(createDto);
            var createdProduct = await _productRepository.CreateAsync(product);
            return _mapper.Map<ProductResponseDto>(createdProduct);
        }

        public async Task<ProductResponseDto> UpdateAsync(string publicId, UpdateProductDto updateDto)
        {
            var product = await _productRepository.GetByPublicIdAsync(publicId);
            _mapper.Map(updateDto, product);
            await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<PaginatedResponseDto<ProductResponseDto>> GetFilteredAsync(ProductFilterRequestDto filter)
        {
            var products = await _productRepository.GetAllAsync();

            if (filter.IsInStock.HasValue)
            {
                products = products.Where(p => (filter.IsInStock.Value && p.Quantity > 0) || 
                    (!filter.IsInStock.Value && p.Quantity == 0));
            }

            if (!string.IsNullOrEmpty(filter.NameStartsWith))
            {
                products = products.Where(p => p.Name.StartsWith(filter.NameStartsWith, 
                    StringComparison.OrdinalIgnoreCase));
            }

            products = filter.OrderByDescending
                ? products.OrderByDescending(p => p.Price).ThenByDescending(x => x.Id)
                : products.OrderBy(p => p.Price).ThenByDescending(x => x.Id);

            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var paginatedProducts = products
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(paginatedProducts);

            return new PaginatedResponseDto<ProductResponseDto>
            {
                Items = productDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = filter.PageNumber > 1,
                HasNextPage = filter.PageNumber < totalPages
            };
        }
    }
}
