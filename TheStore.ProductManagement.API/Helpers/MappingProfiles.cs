using AutoMapper;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Helpers;

public class MappingProfiles:Profile
{
    public MappingProfiles()
    {
        CreateMap<DbResults<Product[]>, Error>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => new List<string?> { src.Message }));
    }
}
