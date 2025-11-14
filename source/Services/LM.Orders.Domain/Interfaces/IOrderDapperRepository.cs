using LM.SharedKernel.Dtos;

namespace LM.Orders.Domain.Interfaces
{
    public interface IOrderDapperRepository
    {
        Task<OrderReadItem?> GetOrderByIdAsync(Guid id);
    }
}