namespace SunnyHillStore.Model.Models.Products
{
    public class ProductFilterRequestDto : BaseFilterRequestDto
    {
        public bool? IsInStock { get; set; }
        public string? NameStartsWith { get; set; }
        public bool OrderByDescending { get; set; }
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
    }
} 