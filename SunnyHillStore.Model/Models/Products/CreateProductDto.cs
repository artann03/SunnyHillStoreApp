using Microsoft.AspNetCore.Http;

namespace SunnyHillStore.Model.Models.Products
{
    public class CreateProductDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }
} 