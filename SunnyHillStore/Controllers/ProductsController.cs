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
            var productDtos = await _productService.GetAllAsync();
            return Ok(productDtos);
        }

        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductFilterRequestDto filter)
        {
            var response = await _productService.GetFilteredAsync(filter);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            var productDto = await _productService.GetByIdAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }
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
                model.ImageUrl = await _cloudinaryService.UploadImageAsync(model.Image);
                var productDto = await _productService.CreateAsync(model);
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
            var productDto = await _productService.UpdateAsync(id, model);
            if (productDto == null)
            {
                return NotFound();
            }
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
