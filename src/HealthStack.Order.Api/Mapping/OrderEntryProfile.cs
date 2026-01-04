using AutoMapper;
using HealthStack.Order.Api.DTOs;
using HealthStack.Order.Api.Models;

namespace HealthStack.Order.Api.Mapping;
public class OrderEntryProfile : Profile
{
    public OrderEntryProfile()
    {
        CreateMap<OrderReadDto, OrderEntry>();

        CreateMap<OrderEntry, OrderReadDto>();
        CreateMap<OrderItem, OrderItemReadDto>();

        CreateMap<OrderCreateDto, OrderEntry>();
        CreateMap<OrderItemCreateDto, OrderItem>();
        // CreateMap<OrderEntryUpdateDto, OrderEntry>();
    }
}