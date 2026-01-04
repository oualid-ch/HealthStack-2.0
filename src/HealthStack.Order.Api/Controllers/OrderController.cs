using AutoMapper;
using FluentValidation;
using Gridify;
using HealthStack.Order.Api.DTOs;

// using HealthStack.Order.Api.DTOs;
using HealthStack.Order.Api.Models;
using HealthStack.Order.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Order.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(
    IValidator<OrderCreateDto> orderCreateValidator,
    IOrderService orderService,
    IMapper mapper
) : ControllerBase
{
    private readonly IValidator<OrderCreateDto> _orderCreateValidator = orderCreateValidator;
    private readonly IOrderService _orderService = orderService;
    private readonly IMapper _mapper = mapper;

    // GET ORDER BY ID
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<OrderReadDto>> GetOrderById(Guid id)
    {
        var order = await _orderService.GetMyOrderByIdAsync(id);
        return Ok(_mapper.Map<OrderReadDto>(order));
    }

    // GET ALL ORDERS
    [HttpGet]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetMyOrders([FromQuery] GridifyQuery query)
    {
        var pagedOrders = await _orderService.GetMyOrdersAsync(query);
        var result = new {
            pagedOrders.Count,
            Data = _mapper.Map<IEnumerable<OrderReadDto>>(pagedOrders.Data)
        };
        return Ok(result);
    }
    
    // POST ORDER
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto orderCreateDto)
    {
        var validation = _orderCreateValidator.Validate(orderCreateDto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        OrderEntry order = _mapper.Map<OrderEntry>(orderCreateDto);
        var created =  await _orderService.CreateOrderAsync(order);

        return CreatedAtAction(nameof(GetOrderById), new { id = created.Id }, _mapper.Map<OrderReadDto>(created));
    }
}