using MediatR;
using LM.Orders.Contracts.Orders.Responses;
using LM.SharedKernel.Dtos;

namespace LM.Orders.Contracts.Orders.Commands
{
    public class CreateOrderCommand : IRequest<CreateOrderResponse>
    {
        public required Guid CustomerId { get; init; }
        public required List<CreateOrderItemDto> Items { get; init; }

        public Guid CreatedByUserId { get; private set; }

        public void SetCreatedByUserId(Guid userId)
        {
            CreatedByUserId = userId;
        }
    }
}