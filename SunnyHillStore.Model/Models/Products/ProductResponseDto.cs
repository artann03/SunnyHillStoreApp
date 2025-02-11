namespace SunnyHillStore.Model.Models.Products
{
    public class ProductResponseDto
    {
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
} 