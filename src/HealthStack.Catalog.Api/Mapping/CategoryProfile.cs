using AutoMapper;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;

namespace HealthStack.Catalog.Api.Mapping;
public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryReadDto>();
        CreateMap<CategoryReadDto, Category>();
        CreateMap<CategoryCreateDto, Category>();
    }
}