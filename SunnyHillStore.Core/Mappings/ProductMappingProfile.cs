using AutoMapper;
using SunnyHillStore.Core.Constants;
using SunnyHillStore.Model.Entities;
using SunnyHillStore.Model.Models.Products;

namespace SunnyHillStore.Core.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductResponseDto>();
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Status, opt => 
                    opt.MapFrom(src => src.Quantity > 0 ? 
                        StatusConstants.InStock.ToString() : 
                        StatusConstants.OutOfStock.ToString()));
            CreateMap<UpdateProductDto, Product>();
        }
    }
} 