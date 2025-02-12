using System.Text.Json.Serialization;

namespace SunnyHillStore.Model.Models.Products
{
    public class ProductResponseDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
} 