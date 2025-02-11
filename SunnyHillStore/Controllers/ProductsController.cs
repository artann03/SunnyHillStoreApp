using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Controllers.Base;
using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Constants;
using SunnyHillStore.Core.Services.External;
using SunnyHillStore.Core.Services.Products;
using SunnyHillStore.Model.Entities;
using SunnyHillStore.Model.Models.Products;
using System.Security.Claims;

namespace SunnyHillStore.Controllers
{
    [Authorize]
    public class ProductsController : BaseController<Product>
    {
        private readonly IProductService _productService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;

        public ProductsController(
            IProductService productService,
            ICloudinaryService cloudinaryService,
            ApplicationDbContext context,
            IOrderService orderService) : base(productService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
            _context = context;
            _orderService = orderService;
        }

        [AllowAnonymous]
        public override async Task<IActionResult> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();


            var productDtos = products.Select(p => new ProductResponseDto
            {
                PublicId = p.PublicId,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                Quantity = p.Quantity,
                Status = p.Status
            });

            return Ok(productDtos);
        }
        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductFilterRequestDto filter)
        {
            var products = await _productService.GetAllAsync();

            if (filter.IsInStock.HasValue)
            {
                products = products.Where(p => (filter.IsInStock.Value && p.Quantity > 0) || (!filter.IsInStock.Value && p.Quantity == 0));
            }

            if (!string.IsNullOrEmpty(filter.NameStartsWith))
            {
                products = products.Where(p => p.Name.StartsWith(filter.NameStartsWith, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.OrderByDescending)
            {
                products = products.OrderByDescending(p => p.Price).ThenByDescending(x => x.Id);
            }
            else
            {
                products = products.OrderBy(p => p.Price).ThenByDescending(x => x.Id);
            }

            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var paginatedProducts = products
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var productDtos = paginatedProducts.Select(p => new ProductResponseDto
            {
                PublicId = p.PublicId,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                Quantity = p.Quantity,
                Status = p.Status
            });

            var response = new PaginatedResponseDto<ProductResponseDto>
            {
                Items = productDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = filter.PageNumber > 1,
                HasNextPage = filter.PageNumber < totalPages
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductResponseDto
            {
                PublicId = product.PublicId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = product.Quantity,
                Status = product.Status
            };

            return Ok(productDto);
        }

        [HttpPost]
        [Authorize(Roles = AuthorizationConstants.AdminRole)]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductDto model)
        {
            if (model.Image == null)
            {
                return BadRequest(new { message = "Image is required" });
            }

            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(model.Image);

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = imageUrl,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    Status = model.Quantity > 0 ? StatusConstants.InStock.ToString() : StatusConstants.OutOfStock.ToString()
                };

                var createdProduct = await _productService.CreateAsync(product);
                var productDto = new ProductResponseDto
                {
                    PublicId = createdProduct.PublicId,
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    ImageUrl = createdProduct.ImageUrl,
                    Price = createdProduct.Price,
                    Quantity = createdProduct.Quantity,
                    Status = createdProduct.Status
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error uploading image: {ex.Message}" });
            }
        }

        [HttpPut()]
        [Authorize(Roles = AuthorizationConstants.AdminRole)]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateProductDto model)
        {
            var product = await _productService.GetByPublicIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.ImageUrl = model.ImageUrl;
            product.Price = model.Price;
            product.Quantity = model.Quantity;

            var updatedProduct = await _productService.UpdateAsync(product);

            var productDto = new ProductResponseDto
            {
                PublicId = updatedProduct.PublicId,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                ImageUrl = updatedProduct.ImageUrl,
                Price = updatedProduct.Price,
                Quantity = updatedProduct.Quantity,
                Status = updatedProduct.Status
            };

            return Ok(productDto);
        }

        [HttpDelete()]
        [Authorize(Roles = AuthorizationConstants.AdminRole)]
        public override async Task<IActionResult> DeleteAsync(string id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyProduct([FromBody] PurchaseProductDto purchaseDto)
        {
            var product = await _productService.GetByPublicIdAsync(purchaseDto.PublicId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            if (product.Quantity < purchaseDto.Quantity)
            {
                return BadRequest(new { message = "Not enough items in stock" });
            }

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = purchaseDto.Quantity,
                UnitPrice = product.Price
            };

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = await _orderService.CreateOrderAsync(userId, new[] { orderItem });

            product.Quantity -= purchaseDto.Quantity;
            if (product.Quantity == 0)
            {
                product.Status = StatusConstants.OutOfStock.ToString();
            }

            await _productService.UpdateAsync(product);

            var response = new PurchaseResponseDto
            {
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                RemainingQuantity = product.Quantity,
                ProductStatus = product.Status
            };

            return Ok(response);
        }
    }
}
