using AutoMapper;
using LM.Orders.Contracts.Orders.Responses;
using LM.Orders.Domain.Aggregates.OrderAggregate;
using LM.SharedKernel.Dtos; 

namespace LM.Orders.Application.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderReadItem, OrderResponse>();
            CreateMap<Order, OrderResponse>();
            CreateMap<OrderItem, OrderItemResponse>();
            CreateMap<Order, Order>();
            CreateMap<OrderItem, OrderItem>();
        }
    }
}