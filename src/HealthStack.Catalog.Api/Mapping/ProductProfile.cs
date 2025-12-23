using AutoMapper;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;

namespace HealthStack.Catalog.Api.Mapping;
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductReadDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<ProductReadDto, Product>();
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
    }
}