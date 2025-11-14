using MediatR;
using AutoMapper;
using LM.Orders.Contracts.Orders.Queries;
using LM.Orders.Contracts.Orders.Responses;
using LM.Orders.Domain.Interfaces;

namespace LM.Orders.Application.QueryHandlers
{
    public class GetOrderQueryHandler(IOrderDapperRepository orderDapperRepository, IOrderItemRepository orderItemRepository, IOrderCacheService orderCacheService, IMapper mapper) : IRequestHandler<GetOrderQuery, OrderResponse?>
    {
        private readonly IOrderDapperRepository _orderDapperRepository = orderDapperRepository;
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IOrderCacheService _orderCacheService = orderCacheService;
        private readonly IMapper _mapper = mapper;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

        public async Task<OrderResponse?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var cachedResponse = await _orderCacheService.GetAsync(request.Id);
            if (cachedResponse != null)
            {
                return cachedResponse;
            }

            var orderItem = await _orderDapperRepository.GetOrderByIdAsync(request.Id);
            if (orderItem == null)
            {
                return null;
            }

            var items = await _orderItemRepository.GetItemsByOrderIdAsync(request.Id);

            var response = _mapper.Map<OrderResponse>(orderItem);
            response.Items = _mapper.Map<List<OrderItemResponse>>(items);

            await _orderCacheService.SetAsync(response, CacheDuration);

            return response;
        }
    }
}