using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Controllers.Base;
using SunnyHillStore.Core.Services.Products;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Controllers
{
    public class ProductsController : BaseController<Product>
    {
        public ProductsController(IProductService productService) : base(productService)
        {
        }

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }
        [HttpPut]
        public override async Task<IActionResult> UpdateAsync(int id, Product product)
        {
            return await base.UpdateAsync(id, product);
        }
        [HttpDelete]
        public override async Task<IActionResult> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }
    }
}
