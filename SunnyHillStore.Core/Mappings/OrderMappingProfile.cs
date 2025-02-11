using AutoMapper;
using SunnyHillStore.Model.Entities;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<Order, MonthlyRevenue>()
            .ForMember(dest => dest.Month, opt => opt.MapFrom(src => $"{src.CreatedAt.Year}-{src.CreatedAt.Month:00}"))
            .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.TotalAmount));

        CreateMap<OrderItem, ProductSaleMetric>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.SalesCount, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
    }
} 