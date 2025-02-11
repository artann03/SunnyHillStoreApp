﻿namespace SunnyHillStore.Model.Models.Products
{
    public class CreateProductModel : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
