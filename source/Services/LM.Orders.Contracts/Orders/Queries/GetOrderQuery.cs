using MediatR;
using LM.Orders.Contracts.Orders.Responses;

namespace LM.Orders.Contracts.Orders.Queries
{
    public class GetOrderQuery(Guid id, Guid? userId) : IRequest<OrderResponse>
    {
        public Guid Id { get; init; } = id;
        public Guid? UserId { get; init; } = userId; 
    }
}