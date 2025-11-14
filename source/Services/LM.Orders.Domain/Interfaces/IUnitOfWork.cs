namespace LM.Orders.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        Task CommitAsync();
        Task RollbackAsync();
    }
}