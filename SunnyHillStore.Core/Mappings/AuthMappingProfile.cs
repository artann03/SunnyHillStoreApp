using AutoMapper;
using SunnyHillStore.Model.Entities;
using SunnyHillStore.Model.Models.Users;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterModel, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleConstants.StandardUser));

        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());

        CreateMap<User, UserProfileResponseDto>();
    }
} 